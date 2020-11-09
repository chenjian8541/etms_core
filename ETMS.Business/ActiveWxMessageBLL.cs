using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Interaction.Output;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ActiveWxMessageBLL : IActiveWxMessageBLL
    {
        private readonly IActiveWxMessageDAL _activeWxMessageDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IClassDAL _classDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        public ActiveWxMessageBLL(IActiveWxMessageDAL activeWxMessageDAL, IEventPublisher eventPublisher,
            IUserOperationLogDAL userOperationLogDAL, IUserDAL userDAL, IStudentDAL studentDAL, IClassDAL classDAL,
            ITempDataCacheDAL tempDataCacheDAL)
        {
            this._activeWxMessageDAL = activeWxMessageDAL;
            this._eventPublisher = eventPublisher;
            this._userOperationLogDAL = userOperationLogDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._classDAL = classDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _activeWxMessageDAL, _userOperationLogDAL, _userDAL, _studentDAL, _classDAL);
        }

        public async Task<ResponseBase> ActiveWxMessageAdd(ActiveWxMessageAddRequest request)
        {
            var now = DateTime.Now;
            var limitBucket = _tempDataCacheDAL.GetWxMessageLimitBucket(request.LoginTenantId, now);
            if (limitBucket != null && limitBucket.TotalCount >= 5)
            {
                return ResponseBase.CommonError("每天最多发送5次微信通知");
            }
            var entity = new EtActiveWxMessage()
            {
                ConfirmCount = 0,
                CreateUserId = request.LoginUserId,
                IsDeleted = EmIsDeleted.Normal,
                IsNeedConfirm = request.IsNeedConfirm,
                MessageContent = request.MessageContent,
                Ot = now,
                ReadCount = 0,
                RelatedIds = EtmsHelper.GetMuIds(request.RelatedIds),
                TenantId = request.LoginTenantId,
                Title = request.Title,
                TotalCount = 0,
                Type = request.Type
            };
            await _activeWxMessageDAL.AddActiveWxMessage(entity);
            _eventPublisher.Publish(new ActiveWxMessageAddEvent(request.LoginTenantId)
            {
                WxMessageAddId = entity.Id,
                CreateTime = now
            });

            var totalCount = limitBucket == null ? 0 : limitBucket.TotalCount;
            _tempDataCacheDAL.SetWxMessageLimitBucket(request.LoginTenantId, now, totalCount++);

            await _userOperationLogDAL.AddUserLog(request, $"发送微信通知:{request.Title}", EmUserOperationType.WxMessage, now);
            return ResponseBase.Success();
        }

        public async Task ActiveWxMessageAddConsumerEvent(ActiveWxMessageAddEvent request)
        {
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(request.WxMessageAddId);
            if (wxMessage == null)
            {
                LOG.Log.Error($"[ActiveWxMessageAddConsumerEvent]微信通知不存在，Id:{request.WxMessageAddId}", this.GetType());
                return;
            }
            switch (wxMessage.Type)
            {
                case EmActiveWxMessageType.AllStudent:
                    await ActiveWxMessageAddConsumerEventAllStudent(wxMessage);
                    break;
                case EmActiveWxMessageType.Class:
                    await ActiveWxMessageAddConsumerEventClass(wxMessage);
                    break;
                case EmActiveWxMessageType.Student:
                    await ActiveWxMessageAddConsumerEventStudent(wxMessage);
                    break;
            }
        }

        private async Task ActiveWxMessageAddConsumerEventAllStudent(EtActiveWxMessage wxMessage)
        {
            var request = new GetAllStudentPagingRequest()
            {
                LoginTenantId = wxMessage.TenantId,
                LoginUserId = wxMessage.CreateUserId,
                PageCurrent = 1,
                PageSize = 100
            };
            var studentPagingData = await _studentDAL.GetAllStudentPaging(request);
            if (studentPagingData.Item2 == 0)
            {
                return;
            }
            await ActiveWxMessageAddConsumerEventAllStudentHandle(wxMessage, studentPagingData.Item1);
            var totalPage = EtmsHelper.GetTotalPage(studentPagingData.Item2, request.PageSize);
            request.PageCurrent++;
            while (request.PageCurrent <= totalPage)
            {
                LOG.Log.Info($"[ActiveWxMessageAddConsumerEventAllStudent]处理第{request.PageCurrent}页的数据", this.GetType());
                var studentResult = await _studentDAL.GetAllStudentPaging(request);
                ActiveWxMessageAddConsumerEventAllStudentHandle(wxMessage, studentResult.Item1).Wait();
                request.PageCurrent++;
            }
        }

        private async Task ActiveWxMessageAddConsumerEventAllStudentHandle(EtActiveWxMessage wxMessage, IEnumerable<GetAllStudentPagingOutput> students)
        {
            var totalCount = 0;
            var wxMessageDetailList = new List<EtActiveWxMessageDetail>();
            var studentIds = new List<long>();
            foreach (var myStudent in students)
            {
                totalCount++;
                studentIds.Add(myStudent.Id);
                wxMessageDetailList.Add(new EtActiveWxMessageDetail()
                {
                    ConfirmOt = null,
                    CreateUserId = wxMessage.CreateUserId,
                    IsConfirm = EmBool.False,
                    IsDeleted = EmIsDeleted.Normal,
                    IsNeedConfirm = wxMessage.IsNeedConfirm,
                    Ot = wxMessage.Ot,
                    StudentId = myStudent.Id,
                    TenantId = wxMessage.TenantId,
                    Title = wxMessage.Title,
                    WxMessageId = wxMessage.Id
                });
            }
            if (wxMessageDetailList.Count == 0)
            {
                LOG.Log.Warn($"[ActiveWxMessageAddConsumerEventAllStudentHandle]未查询到需要接收通知的学员信息,TenantId:{wxMessage.Id}", this.GetType());
            }
            _activeWxMessageDAL.AddActiveWxMessageDetail(wxMessageDetailList);
            await _activeWxMessageDAL.AddActiveWxMessageCount(wxMessage.Id, totalCount);
            _eventPublisher.Publish(new NoticeStudentsOfWxMessageEvent(wxMessage.TenantId)
            {
                WxMessageAddId = wxMessage.Id,
                StudentIds = studentIds,
                Ot = wxMessage.Ot
            });
        }

        private async Task ActiveWxMessageAddConsumerEventClass(EtActiveWxMessage wxMessage)
        {
            var totalCount = 0;
            var wxMessageDetailList = new List<EtActiveWxMessageDetail>();
            var studentIds = new List<long>();
            var strClassIds = wxMessage.RelatedIds.Split(',');
            foreach (var strClassId in strClassIds)
            {
                if (string.IsNullOrEmpty(strClassId))
                {
                    continue;
                }
                var myClassId = strClassId.ToLong();
                var myClassBucket = await _classDAL.GetClassBucket(myClassId);
                if (myClassBucket == null || myClassBucket.EtClass == null)
                {
                    LOG.Log.Warn($"[ActiveWxMessageAddConsumerEventClass]班级不存在,TenantId:{wxMessage.TenantId},ClassId:{myClassId}", this.GetType());
                    continue;
                }
                if (myClassBucket.EtClassStudents != null && myClassBucket.EtClassStudents.Count > 0)
                {
                    foreach (var myClassStudent in myClassBucket.EtClassStudents)
                    {
                        var myId = myClassStudent.StudentId;
                        var myStudent = await _studentDAL.GetStudent(myId);
                        if (myStudent == null || myStudent.Student == null)
                        {
                            LOG.Log.Warn($"[ActiveWxMessageAddConsumerEventClass]学员不存在,TenantId:{wxMessage.TenantId},StudentId:{myId}", this.GetType());
                            continue;
                        }
                        totalCount++;
                        studentIds.Add(myId);
                        wxMessageDetailList.Add(new EtActiveWxMessageDetail()
                        {
                            ConfirmOt = null,
                            CreateUserId = wxMessage.CreateUserId,
                            IsConfirm = EmBool.False,
                            IsDeleted = EmIsDeleted.Normal,
                            IsNeedConfirm = wxMessage.IsNeedConfirm,
                            Ot = wxMessage.Ot,
                            StudentId = myId,
                            TenantId = wxMessage.TenantId,
                            Title = wxMessage.Title,
                            WxMessageId = wxMessage.Id
                        });
                    }
                }
            }
            if (wxMessageDetailList.Count == 0)
            {
                LOG.Log.Warn($"[ActiveWxMessageAddConsumerEventClass]未查询到需要接收通知的学员信息,TenantId:{wxMessage.Id}", this.GetType());
            }
            _activeWxMessageDAL.AddActiveWxMessageDetail(wxMessageDetailList);
            await _activeWxMessageDAL.AddActiveWxMessageCount(wxMessage.Id, totalCount);
            _eventPublisher.Publish(new NoticeStudentsOfWxMessageEvent(wxMessage.TenantId)
            {
                WxMessageAddId = wxMessage.Id,
                StudentIds = studentIds,
                Ot = wxMessage.Ot
            });
        }

        private async Task ActiveWxMessageAddConsumerEventStudent(EtActiveWxMessage wxMessage)
        {
            var totalCount = 0;
            var wxMessageDetailList = new List<EtActiveWxMessageDetail>();
            var studentIds = new List<long>();
            var strStudentIds = wxMessage.RelatedIds.Split(',');
            foreach (var strId in strStudentIds)
            {
                if (string.IsNullOrEmpty(strId))
                {
                    continue;
                }
                var myId = strId.ToLong();
                var myStudent = await _studentDAL.GetStudent(myId);
                if (myStudent == null || myStudent.Student == null)
                {
                    LOG.Log.Warn($"[ActiveWxMessageAddConsumerEventStudent]学员不存在,TenantId:{wxMessage.TenantId},StudentId:{myId}", this.GetType());
                    continue;
                }
                totalCount++;
                studentIds.Add(myId);
                wxMessageDetailList.Add(new EtActiveWxMessageDetail()
                {
                    ConfirmOt = null,
                    CreateUserId = wxMessage.CreateUserId,
                    IsConfirm = EmBool.False,
                    IsDeleted = EmIsDeleted.Normal,
                    IsNeedConfirm = wxMessage.IsNeedConfirm,
                    Ot = wxMessage.Ot,
                    StudentId = myId,
                    TenantId = wxMessage.TenantId,
                    Title = wxMessage.Title,
                    WxMessageId = wxMessage.Id
                });
            }
            if (wxMessageDetailList.Count == 0)
            {
                LOG.Log.Warn($"[ActiveWxMessageAddConsumerEventStudent]未查询到需要接收通知的学员信息,TenantId:{wxMessage.Id}", this.GetType());
            }
            _activeWxMessageDAL.AddActiveWxMessageDetail(wxMessageDetailList);
            await _activeWxMessageDAL.AddActiveWxMessageCount(wxMessage.Id, totalCount);
            _eventPublisher.Publish(new NoticeStudentsOfWxMessageEvent(wxMessage.TenantId)
            {
                WxMessageAddId = wxMessage.Id,
                StudentIds = studentIds,
                Ot = wxMessage.Ot
            });
        }

        public async Task<ResponseBase> ActiveWxMessageEdit(ActiveWxMessageEditRequest request)
        {
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(request.CId);
            if (wxMessage == null)
            {
                return ResponseBase.CommonError("微信通知不存在");
            }
            wxMessage.Title = request.Title;
            wxMessage.MessageContent = request.MessageContent;
            wxMessage.IsNeedConfirm = request.IsNeedConfirm;
            await _activeWxMessageDAL.EditActiveWxMessage(wxMessage);
            await _activeWxMessageDAL.SyncWxMessageDetail(request.CId, request.Title, request.IsNeedConfirm);

            await _userOperationLogDAL.AddUserLog(request, $"编辑微信通知:{request.Title}", EmUserOperationType.WxMessage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveWxMessageDel(ActiveWxMessageDelRequest request)
        {
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(request.CId);
            if (wxMessage == null)
            {
                return ResponseBase.CommonError("微信通知不存在");
            }
            await _activeWxMessageDAL.DelActiveWxMessage(request.CId);

            await _userOperationLogDAL.AddUserLog(request, $"删除微信通知:{wxMessage.Title}", EmUserOperationType.WxMessage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveWxMessageGet(ActiveWxMessageGetRequest request)
        {
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(request.CId);
            if (wxMessage == null)
            {
                return ResponseBase.CommonError("微信通知不存在");
            }
            var user = await _userDAL.GetUser(wxMessage.CreateUserId);
            var relatedDesc = string.Empty;
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            switch (wxMessage.Type)
            {
                case EmActiveWxMessageType.Class:
                    relatedDesc = await ComBusiness.GetClassNames(tempBoxClass, _classDAL, wxMessage.RelatedIds);
                    break;
                case EmActiveWxMessageType.Student:
                    relatedDesc = await ComBusiness.GetStudentNames(tempBoxStudent, _studentDAL, wxMessage.RelatedIds);
                    break;
                case EmActiveWxMessageType.AllStudent:
                    relatedDesc = "全体学员";
                    break;
            }
            var output = new ActiveWxMessageGetOutput()
            {
                CId = wxMessage.Id,
                IsNeedConfirm = wxMessage.IsNeedConfirm,
                ConfirmCount = wxMessage.ConfirmCount,
                CreateUserId = wxMessage.CreateUserId,
                CreateUserName = user.Name,
                MessageContent = wxMessage.MessageContent,
                ReadCount = wxMessage.ReadCount,
                Ot = wxMessage.Ot,
                Title = wxMessage.Title,
                TotalCount = wxMessage.TotalCount,
                Type = wxMessage.Type,
                TypeDesc = EmActiveWxMessageType.GetWxMessageTypeDesc(wxMessage.Type),
                RelatedDesc = relatedDesc
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActiveWxMessageGetPaging(ActiveWxMessageGetPagingRequest request)
        {
            var pagingData = await _activeWxMessageDAL.GetPaging(request);
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var output = new List<ActiveWxMessageGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                var relatedDesc = string.Empty;
                switch (p.Type)
                {
                    case EmActiveWxMessageType.Class:
                        relatedDesc = await ComBusiness.GetClassNames(tempBoxClass, _classDAL, p.RelatedIds);
                        break;
                    case EmActiveWxMessageType.Student:
                        relatedDesc = await ComBusiness.GetStudentNames(tempBoxStudent, _studentDAL, p.RelatedIds);
                        break;
                    case EmActiveWxMessageType.AllStudent:
                        relatedDesc = "全体学员";
                        break;
                }
                var userName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.CreateUserId);
                output.Add(new ActiveWxMessageGetPagingOutput()
                {
                    CId = p.Id,
                    IsNeedConfirm = p.IsNeedConfirm,
                    ConfirmCount = p.ConfirmCount,
                    CreateUserId = p.CreateUserId,
                    CreateUserName = userName,
                    ReadCount = p.ReadCount,
                    Ot = p.Ot,
                    Title = p.Title,
                    TotalCount = p.TotalCount,
                    Type = p.Type,
                    TypeDesc = EmActiveWxMessageType.GetWxMessageTypeDesc(p.Type),
                    RelatedDesc = relatedDesc
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActiveWxMessageGetPagingOutput>(pagingData.Item2, output));
        }
    }
}