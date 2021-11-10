using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Interaction.Output;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.Utility;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using ETMS.Entity.Config;

namespace ETMS.Business
{
    public class ActiveGrowthRecordBLL : IActiveGrowthRecordBLL
    {
        private readonly IActiveGrowthRecordDAL _activeGrowthRecordDAL;

        private readonly IUserDAL _userDAL;

        private readonly IClassDAL _classDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IStudentGrowingTagDAL _studentGrowingTagDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        public ActiveGrowthRecordBLL(IActiveGrowthRecordDAL activeGrowthRecordDAL, IUserDAL userDAL, IClassDAL classDAL,
           IStudentDAL studentDAL, IStudentGrowingTagDAL studentGrowingTagDAL, IUserOperationLogDAL userOperationLogDAL,
           IEventPublisher eventPublisher, IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._activeGrowthRecordDAL = activeGrowthRecordDAL;
            this._userDAL = userDAL;
            this._classDAL = classDAL;
            this._studentDAL = studentDAL;
            this._studentGrowingTagDAL = studentGrowingTagDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._eventPublisher = eventPublisher;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _activeGrowthRecordDAL, _userDAL, _classDAL, _studentDAL, _studentGrowingTagDAL, _userOperationLogDAL);
        }

        private List<string> GetMediasUrl(string workMedias)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(workMedias))
            {
                return result;
            }
            var myMedias = workMedias.Split('|');
            foreach (var p in myMedias)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    result.Add(UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p));
                }
            }
            return result;
        }

        public async Task<ResponseBase> ActiveGrowthRecordClassGetPaging(ActiveGrowthRecordClassGetPagingRequest request)
        {
            var pagingData = await _activeGrowthRecordDAL.GetPaging(request);
            var output = new List<ActiveGrowthRecordGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var allGrowingTag = await _studentGrowingTagDAL.GetAllStudentGrowingTag();
            foreach (var p in pagingData.Item1)
            {
                var user = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.CreateUserId);
                var myGrowingTag = allGrowingTag.FirstOrDefault(j => j.Id == p.GrowingTag);
                output.Add(new ActiveGrowthRecordGetPagingOutput()
                {
                    RelatedIds = p.RelatedIds,
                    CreateUserId = p.CreateUserId,
                    CreateUserName = user.Name,
                    GrowingTag = p.GrowingTag,
                    GrowingTagDesc = myGrowingTag?.Name,
                    GrowthRecordId = p.Id,
                    Ot = p.Ot,
                    RelatedDesc = await ComBusiness.GetClassNames(tempBoxClass, _classDAL, p.RelatedIds),
                    SendType = p.SendType,
                    Type = p.Type,
                    GrowthContent = p.GrowthContent,
                    TypeDesc = EmActiveGrowthRecordType.GetActiveGrowthRecordTypeDesc(p.Type),
                    FavoriteStatus = p.FavoriteStatus,
                    ReadStatus = p.ReadStatus,
                    ReadCount = p.ReadCount,
                    TotalCount = p.TotalCount
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActiveGrowthRecordGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ActiveGrowthRecordStudentGetPaging(ActiveGrowthRecordStudentGetPagingRequest request)
        {
            var pagingData = await _activeGrowthRecordDAL.GetPaging(request);
            var output = new List<ActiveGrowthRecordGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            var allGrowingTag = await _studentGrowingTagDAL.GetAllStudentGrowingTag();
            foreach (var p in pagingData.Item1)
            {
                var user = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.CreateUserId);
                var myGrowingTag = allGrowingTag.FirstOrDefault(j => j.Id == p.GrowingTag);
                output.Add(new ActiveGrowthRecordGetPagingOutput()
                {
                    RelatedIds = p.RelatedIds,
                    CreateUserId = p.CreateUserId,
                    CreateUserName = user.Name,
                    GrowingTag = p.GrowingTag,
                    GrowingTagDesc = myGrowingTag?.Name,
                    GrowthRecordId = p.Id,
                    Ot = p.Ot,
                    RelatedDesc = await ComBusiness.GetStudentNames(tempBoxStudent, _studentDAL, p.RelatedIds),
                    SendType = p.SendType,
                    Type = p.Type,
                    GrowthContent = p.GrowthContent,
                    TypeDesc = EmActiveGrowthRecordType.GetActiveGrowthRecordTypeDesc(p.Type),
                    FavoriteStatus = p.FavoriteStatus,
                    ReadStatus = p.ReadStatus,
                    ReadCount = p.ReadCount,
                    TotalCount = p.TotalCount
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActiveGrowthRecordGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ActiveGrowthRecordStudentStatusGet(ActiveGrowthRecordStudentStatusGetRequest request)
        {
            var output = new ActiveGrowthRecordStudentStatusGetOutput()
            {
                ReadList = new List<ActiveGrowthRecordStudentStatusItem>(),
                UnReadList = new List<ActiveGrowthRecordStudentStatusItem>()
            };
            var activeGrowthRecordSimpleView = await _activeGrowthRecordDAL.GetActiveGrowthRecordDetailSimpleView(request.CId);
            if (activeGrowthRecordSimpleView.Any())
            {
                foreach (var p in activeGrowthRecordSimpleView)
                {
                    var myStudentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (myStudentBucket == null || myStudentBucket.Student == null)
                    {
                        continue;
                    }
                    if (p.ReadStatus == EmBool.True)
                    {
                        output.ReadList.Add(new ActiveGrowthRecordStudentStatusItem()
                        {
                            ReadStatus = p.ReadStatus,
                            FavoriteStatus = p.FavoriteStatus,
                            Id = p.Id,
                            StudentName = myStudentBucket.Student.Name
                        });
                    }
                    else
                    {
                        output.UnReadList.Add(new ActiveGrowthRecordStudentStatusItem()
                        {
                            ReadStatus = p.ReadStatus,
                            FavoriteStatus = p.FavoriteStatus,
                            Id = p.Id,
                            StudentName = myStudentBucket.Student.Name
                        });
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActiveGrowthRecordClassAdd(ActiveGrowthRecordClassAddRequest request)
        {
            var now = DateTime.Now;
            var growthMedias = string.Empty;
            if (request.GrowthMediasKeys != null && request.GrowthMediasKeys.Count > 0)
            {
                growthMedias = string.Join('|', request.GrowthMediasKeys);
            }
            var classId = request.ClassIds[0];
            var myClassBucket = await _classDAL.GetClassBucket(classId);
            if (myClassBucket == null || myClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var totalCount = 0;
            if (myClassBucket.EtClassStudents != null || myClassBucket.EtClassStudents.Any())
            {
                totalCount = myClassBucket.EtClassStudents.Count;
            }
            var entity = new EtActiveGrowthRecord()
            {
                Type = EmActiveGrowthRecordType.Class,
                CreateUserId = request.LoginUserId,
                TenantId = request.LoginTenantId,
                Ot = now,
                GrowingTag = request.GrowingTag,
                GrowthContent = request.GrowthContent,
                GrowthMedias = growthMedias,
                IsDeleted = EmIsDeleted.Normal,
                SendType = request.SendType,
                RelatedIds = EtmsHelper.GetMuIds(request.ClassIds),
                TotalCount = totalCount
            };
            await _activeGrowthRecordDAL.AddActiveGrowthRecord(entity);
            _eventPublisher.Publish(new ActiveGrowthRecordAddEvent(request.LoginTenantId)
            {
                GrowthRecordId = entity.Id,
                CreateTime = now
            });

            await _userOperationLogDAL.AddUserLog(request, $"新增班级档案-{request.GrowthContent}", EmUserOperationType.ActiveGrowthRecord, now);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveGrowthRecordStudentAdd(ActiveGrowthRecordStudentAddRequest request)
        {
            var now = DateTime.Now;
            var growthMedias = string.Empty;
            if (request.GrowthMediasKeys != null && request.GrowthMediasKeys.Count > 0)
            {
                growthMedias = string.Join('|', request.GrowthMediasKeys);
            }
            var entity = new EtActiveGrowthRecord()
            {
                Type = EmActiveGrowthRecordType.Student,
                CreateUserId = request.LoginUserId,
                TenantId = request.LoginTenantId,
                Ot = now,
                GrowingTag = request.GrowingTag,
                GrowthContent = request.GrowthContent,
                GrowthMedias = growthMedias,
                IsDeleted = EmIsDeleted.Normal,
                SendType = request.SendType,
                RelatedIds = EtmsHelper.GetMuIds(request.StudentIds),
                TotalCount = 1,
                ReadCount = 0
            };
            await _activeGrowthRecordDAL.AddActiveGrowthRecord(entity);
            _eventPublisher.Publish(new ActiveGrowthRecordAddEvent(request.LoginTenantId)
            {
                GrowthRecordId = entity.Id,
                CreateTime = now
            });

            await _userOperationLogDAL.AddUserLog(request, $"新增学员档案-{request.GrowthContent}", EmUserOperationType.ActiveGrowthRecord, now);
            return ResponseBase.Success();
        }

        public async Task ActiveGrowthRecordAddConsumerEvent(ActiveGrowthRecordAddEvent request)
        {
            var activeGrowthRecordBucket = await _activeGrowthRecordDAL.GetActiveGrowthRecord(request.GrowthRecordId);
            if (activeGrowthRecordBucket == null || activeGrowthRecordBucket.ActiveGrowthRecord == null)
            {
                LOG.Log.Error($"新增成长档案：未找到数据,{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var activeGrowthRecord = activeGrowthRecordBucket.ActiveGrowthRecord;
            var activeGrowthRecordDetails = new List<EtActiveGrowthRecordDetail>();
            if (activeGrowthRecord.Type == EmActiveGrowthRecordType.Student)
            {
                var studentIds = activeGrowthRecord.RelatedIds.Split(',');
                foreach (var p in studentIds)
                {
                    if (string.IsNullOrEmpty(p))
                    {
                        continue;
                    }
                    activeGrowthRecordDetails.Add(new EtActiveGrowthRecordDetail()
                    {
                        CreateUserId = activeGrowthRecord.CreateUserId,
                        FavoriteStatus = EmActiveGrowthRecordDetailFavoriteStatus.No,
                        GrowingTag = activeGrowthRecord.GrowingTag,
                        GrowthContent = activeGrowthRecord.GrowthContent,
                        GrowthMedias = activeGrowthRecord.GrowthMedias,
                        GrowthRecordId = activeGrowthRecord.Id,
                        IsDeleted = activeGrowthRecord.IsDeleted,
                        Ot = activeGrowthRecord.Ot,
                        SendType = activeGrowthRecord.SendType,
                        StudentId = p.ToLong(),
                        TenantId = activeGrowthRecord.TenantId
                    });
                }
            }
            else
            {
                var strClassIds = activeGrowthRecord.RelatedIds.Split(',');
                foreach (var classId in strClassIds)
                {
                    if (string.IsNullOrEmpty(classId))
                    {
                        continue;
                    }
                    var myClass = await _classDAL.GetClassBucket(classId.ToLong());
                    if (myClass == null || myClass.EtClassStudents == null || myClass.EtClassStudents.Count == 0)
                    {
                        continue;
                    }
                    foreach (var p in myClass.EtClassStudents)
                    {
                        activeGrowthRecordDetails.Add(new EtActiveGrowthRecordDetail()
                        {
                            CreateUserId = activeGrowthRecord.CreateUserId,
                            FavoriteStatus = EmActiveGrowthRecordDetailFavoriteStatus.No,
                            GrowingTag = activeGrowthRecord.GrowingTag,
                            GrowthContent = activeGrowthRecord.GrowthContent,
                            GrowthMedias = activeGrowthRecord.GrowthMedias,
                            GrowthRecordId = activeGrowthRecord.Id,
                            IsDeleted = activeGrowthRecord.IsDeleted,
                            Ot = activeGrowthRecord.Ot,
                            SendType = activeGrowthRecord.SendType,
                            StudentId = p.StudentId,
                            TenantId = activeGrowthRecord.TenantId
                        });
                    }
                }
            }
            if (activeGrowthRecordDetails.Count > 0)
            {
                await _activeGrowthRecordDAL.UpdateActiveGrowthRecordTotalCount(activeGrowthRecord.Id, activeGrowthRecordDetails.Count);
                _activeGrowthRecordDAL.AddActiveGrowthRecordDetail(activeGrowthRecordDetails);

                _eventPublisher.Publish(new NoticeStudentsOfGrowthRecordEvent(request.TenantId)
                {
                    GrowthRecordId = request.GrowthRecordId
                });
            }
        }

        public async Task<ResponseBase> ActiveGrowthRecordGet(ActiveGrowthRecordGetRequest request)
        {
            var activeGrowthRecordBucket = await _activeGrowthRecordDAL.GetActiveGrowthRecord(request.CId);
            if (activeGrowthRecordBucket == null || activeGrowthRecordBucket.ActiveGrowthRecord == null)
            {
                return ResponseBase.CommonError("成长档案不存在");
            }
            var p = activeGrowthRecordBucket.ActiveGrowthRecord;
            var user = await _userDAL.GetUser(p.CreateUserId);
            var allGrowingTag = await _studentGrowingTagDAL.GetAllStudentGrowingTag();
            var myGrowingTag = allGrowingTag.FirstOrDefault(j => j.Id == p.GrowingTag);
            var relatedDesc = string.Empty;
            if (p.Type == EmActiveGrowthRecordType.Class)
            {
                var tempBoxClass = new DataTempBox<EtClass>();
                relatedDesc = await ComBusiness.GetClassNames(tempBoxClass, _classDAL, p.RelatedIds);
            }
            else
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                relatedDesc = await ComBusiness.GetStudentNames(tempBoxStudent, _studentDAL, p.RelatedIds);
            }
            var tempBoxUser = new DataTempBox<EtUser>();
            var output = new ActiveGrowthRecordGetOutput()
            {
                CreateUserId = p.CreateUserId,
                CreateUserName = user.Name,
                GrowingTag = p.GrowingTag,
                GrowingTagDesc = myGrowingTag?.Name,
                GrowthContent = p.GrowthContent,
                GrowthMediasUrl = GetMediasUrl(p.GrowthMedias),
                GrowthRecordId = p.Id,
                Ot = p.Ot,
                RelatedIds = p.RelatedIds,
                Type = p.Type,
                TypeDesc = EmActiveGrowthRecordType.GetActiveGrowthRecordTypeDesc(p.Type),
                SendType = p.SendType,
                RelatedDesc = relatedDesc,
                CommentOutputs = await GetCommentOutput(activeGrowthRecordBucket.Comments, tempBoxUser)
            };
            return ResponseBase.Success(output);
        }

        private async Task<List<CommentOutput>> GetCommentOutput(List<EtActiveGrowthRecordDetailComment> activeGrowthRecordDetailComments, DataTempBox<EtUser> tempBoxUser)
        {
            var commentOutputs = new List<CommentOutput>();
            if (activeGrowthRecordDetailComments != null || activeGrowthRecordDetailComments.Count > 0)
            {
                var first = activeGrowthRecordDetailComments.Where(j => j.ReplyId == null).OrderBy(j => j.Ot);
                foreach (var myFirstComment in first)
                {
                    var firstrelatedManName = string.Empty;
                    var firstrelatedManAvatar = string.Empty;
                    if (myFirstComment.SourceType == EmActiveCommentSourceType.User)
                    {
                        var myRelatedUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, myFirstComment.SourceId);
                        if (myRelatedUser != null)
                        {
                            firstrelatedManName = myRelatedUser.Name;
                            firstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                        }
                    }
                    else
                    {
                        firstrelatedManName = myFirstComment.Nickname;
                        firstrelatedManAvatar = myFirstComment.Headimgurl;
                    }
                    commentOutputs.Add(new CommentOutput()
                    {
                        CommentContent = myFirstComment.CommentContent,
                        CommentId = myFirstComment.Id,
                        Ot = myFirstComment.Ot.EtmsToMinuteString(),
                        ReplyId = myFirstComment.ReplyId,
                        SourceType = myFirstComment.SourceType,
                        RelatedManAvatar = firstrelatedManAvatar,
                        RelatedManName = firstrelatedManName,
                        OtDesc = EtmsHelper.GetOtFriendlyDesc(myFirstComment.Ot)
                    });
                    var second = activeGrowthRecordDetailComments.Where(p => p.ReplyId == myFirstComment.Id).OrderBy(j => j.Ot);
                    foreach (var mySecondComment in second)
                    {
                        var secondfirstrelatedManName = string.Empty;
                        var secondfirstrelatedManAvatar = string.Empty;
                        if (mySecondComment.SourceType == EmActiveCommentSourceType.User)
                        {
                            var myRelatedUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, mySecondComment.SourceId);
                            if (myRelatedUser != null)
                            {
                                secondfirstrelatedManName = myRelatedUser.Name;
                                secondfirstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                            }
                        }
                        else
                        {
                            secondfirstrelatedManName = mySecondComment.Nickname;
                            secondfirstrelatedManAvatar = mySecondComment.Headimgurl;
                        }
                        commentOutputs.Add(new CommentOutput()
                        {
                            CommentContent = mySecondComment.CommentContent,
                            CommentId = mySecondComment.Id,
                            Ot = mySecondComment.Ot.EtmsToMinuteString(),
                            OtDesc = EtmsHelper.GetOtFriendlyDesc(mySecondComment.Ot),
                            RelatedManAvatar = secondfirstrelatedManAvatar,
                            RelatedManName = secondfirstrelatedManName,
                            ReplyId = mySecondComment.ReplyId,
                            SourceType = mySecondComment.SourceType,
                            ReplyRelatedManName = firstrelatedManName
                        });
                    }
                }
            }
            return commentOutputs;
        }

        public async Task<ResponseBase> ActiveGrowthRecordDel(ActiveGrowthRecordDelRequest request)
        {
            var activeGrowthRecordBucket = await _activeGrowthRecordDAL.GetActiveGrowthRecord(request.CId);
            if (activeGrowthRecordBucket == null || activeGrowthRecordBucket.ActiveGrowthRecord == null)
            {
                return ResponseBase.CommonError("成长档案不存在");
            }
            await _activeGrowthRecordDAL.DelActiveGrowthRecord(request.CId);
            AliyunOssUtil.DeleteObject2(activeGrowthRecordBucket.ActiveGrowthRecord.GrowthMedias);

            await _userOperationLogDAL.AddUserLog(request, $"删除成长档案-{activeGrowthRecordBucket.ActiveGrowthRecord.GrowthContent}", EmUserOperationType.ActiveGrowthRecord);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveGrowthCommentAdd(ActiveGrowthCommentAddRequest request)
        {
            var comment = new EtActiveGrowthRecordDetailComment()
            {
                CommentContent = request.CommentContent,
                GrowthRecordDetailId = 0,
                GrowthRecordId = request.GrowthRecordId,
                Headimgurl = string.Empty,
                Nickname = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                Ot = DateTime.Now,
                ReplyId = request.ReplyId,
                SourceId = request.LoginUserId,
                SourceType = EmActiveCommentSourceType.User,
                TenantId = request.LoginTenantId
            };
            await _activeGrowthRecordDAL.AddActiveGrowthRecordDetailComment(comment);

            _eventPublisher.Publish(new NoticeStudentActiveGrowthCommentEvent(request.LoginTenantId)
            {
                ActiveGrowthRecordDetailComment = comment
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加成长档案评论-{comment.CommentContent}", EmUserOperationType.ActiveGrowthRecord);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveGrowthCommentDel(ActiveGrowthCommentDelRequest request)
        {
            await _activeGrowthRecordDAL.DelActiveGrowthRecordDetailComment(request.GrowthRecordId, request.CommentId);
            await _userOperationLogDAL.AddUserLog(request, "删除成长档案评论", EmUserOperationType.ActiveGrowthRecord);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveGrowthStudentGetPaging(ActiveGrowthStudentGetPagingRequest request)
        {
            var pagingData = await _activeGrowthRecordDAL.GetDetailPaging(request);
            var output = new List<ActiveGrowthStudentGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                var allstudentGrowingTag = await _studentGrowingTagDAL.GetAllStudentGrowingTag();
                foreach (var p in pagingData.Item1)
                {
                    var myGrowingTag = allstudentGrowingTag.FirstOrDefault(j => j.Id == p.GrowingTag);
                    output.Add(new ActiveGrowthStudentGetPagingOutput()
                    {
                        FavoriteStatus = p.FavoriteStatus,
                        GrowingTag = p.GrowingTag,
                        GrowingTagDesc = myGrowingTag?.Name,
                        GrowthContent = p.GrowthContent,
                        GrowthMediasUrl = GetMediasUrl(p.GrowthMedias),
                        GrowthRecordDetailId = p.Id,
                        GrowthRecordId = p.GrowthRecordId,
                        OtDesc = p.Ot.EtmsToMinuteString(),
                        UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.CreateUserId)
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActiveGrowthStudentGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
