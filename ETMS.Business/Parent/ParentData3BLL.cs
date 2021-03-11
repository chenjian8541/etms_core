using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ParentData3BLL : IParentData3BLL
    {
        private readonly IActiveWxMessageDAL _activeWxMessageDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IActiveWxMessageParentReadDAL _activeWxMessageParentReadDAL;

        private readonly IActiveGrowthRecordDAL _activeGrowthRecordDAL;

        private readonly ITryCalssApplyLogDAL _tryCalssApplyLogDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IAppConfigDAL _appConfigDAL;

        public ParentData3BLL(IActiveWxMessageDAL activeWxMessageDAL, IStudentDAL studentDAL, IActiveWxMessageParentReadDAL activeWxMessageParentReadDAL,
            IActiveGrowthRecordDAL activeGrowthRecordDAL, ITryCalssApplyLogDAL tryCalssApplyLogDAL, IStudentCheckOnLogDAL studentCheckOnLogDAL,
            IEventPublisher eventPublisher, IStudentAccountRechargeDAL studentAccountRechargeDAL, IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL,
            IAppConfigDAL appConfigDAL)
        {
            this._activeWxMessageDAL = activeWxMessageDAL;
            this._studentDAL = studentDAL;
            this._activeWxMessageParentReadDAL = activeWxMessageParentReadDAL;
            this._activeGrowthRecordDAL = activeGrowthRecordDAL;
            this._tryCalssApplyLogDAL = tryCalssApplyLogDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._eventPublisher = eventPublisher;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._appConfigDAL = appConfigDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _activeWxMessageDAL, _studentDAL, _activeWxMessageParentReadDAL, _activeGrowthRecordDAL,
                _tryCalssApplyLogDAL, _studentCheckOnLogDAL, _studentAccountRechargeDAL, _studentAccountRechargeLogDAL, _appConfigDAL);
        }

        public async Task<ResponseBase> WxMessageDetailPaging(WxMessageDetailPagingRequest request)
        {
            var pagingData = await _activeWxMessageDAL.GetDetailPaging(request);
            var output = new List<WxMessageDetailPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new WxMessageDetailPagingOutput()
                {
                    IsConfirm = p.IsConfirm,
                    IsNeedConfirm = p.IsNeedConfirm,
                    IsRead = p.IsRead,
                    OtDesc = p.Ot.EtmsToDateShortString(),
                    StudentId = p.StudentId,
                    Title = p.Title,
                    WxMessageDetailId = p.Id,
                    WxMessageId = p.WxMessageId
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<WxMessageDetailPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> WxMessageDetailGet(WxMessageDetailGetRequest request)
        {
            var wxMessageDetail = await _activeWxMessageDAL.GetActiveWxMessageDetail(request.WxMessageDetailId);
            if (wxMessageDetail == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(wxMessageDetail.WxMessageId);
            if (wxMessage == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }
            var student = await _studentDAL.GetStudent(wxMessageDetail.StudentId);
            if (student == null || student.Student == null)
            {
                return ResponseBase.CommonError("学员信息不存在");
            }
            var output = new WxMessageDetailGetOutput()
            {
                WxMessageId = wxMessageDetail.WxMessageId,
                IsConfirm = wxMessageDetail.IsConfirm,
                IsNeedConfirm = wxMessageDetail.IsNeedConfirm,
                IsRead = wxMessageDetail.IsRead,
                MessageContent = wxMessage.MessageContent,
                Title = wxMessage.Title,
                Ot = wxMessage.Ot,
                StudentId = wxMessageDetail.StudentId,
                WxMessageDetailId = wxMessageDetail.Id,
                StudentName = student.Student.Name
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> WxMessageDetailSetRead(WxMessageDetailSetReadRequest request)
        {
            var wxMessageDetail = await _activeWxMessageDAL.GetActiveWxMessageDetail(request.WxMessageDetailId);
            if (wxMessageDetail == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }
            if (wxMessageDetail.IsRead == EmBool.True)
            {
                LOG.Log.Warn("[WxMessageDetailSetRead]重复提交设置已读请求", this.GetType());
                return ResponseBase.Success();
            }
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(wxMessageDetail.WxMessageId);
            if (wxMessage == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }

            wxMessageDetail.IsRead = EmBool.True;
            await _activeWxMessageDAL.EditActiveWxMessageDetail(wxMessageDetail);

            wxMessage.ReadCount += 1;
            await _activeWxMessageDAL.EditActiveWxMessage(wxMessage);

            await _activeWxMessageParentReadDAL.UpdateParentRead(request.LoginPhone, request.ParentStudentIds);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMessageDetailSetConfirm(WxMessageDetailSetConfirmRequest request)
        {
            var wxMessageDetail = await _activeWxMessageDAL.GetActiveWxMessageDetail(request.WxMessageDetailId);
            if (wxMessageDetail == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }
            if (wxMessageDetail.IsConfirm == EmBool.True)
            {
                LOG.Log.Warn("[WxMessageDetailSetConfirm]重复提交确认请求", this.GetType());
                return ResponseBase.Success();
            }
            var wxMessage = await _activeWxMessageDAL.GetActiveWxMessage(wxMessageDetail.WxMessageId);
            if (wxMessage == null)
            {
                return ResponseBase.CommonError("通知记录不存在");
            }

            wxMessageDetail.IsConfirm = EmBool.True;
            await _activeWxMessageDAL.EditActiveWxMessageDetail(wxMessageDetail);

            wxMessage.ConfirmCount += 1;
            await _activeWxMessageDAL.EditActiveWxMessage(wxMessage);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMessageGetUnreadCount(WxMessageGetUnreadCountRequest request)
        {
            return ResponseBase.Success(await _activeWxMessageParentReadDAL.GetParentUnreadCount(request.LoginPhone, request.ParentStudentIds));
        }

        public async Task<ResponseBase> TryCalssApply(TryCalssApplyRequest request)
        {
            var log = await _activeGrowthRecordDAL.GetActiveGrowthRecordDetail(request.GrowthRecordDetailId);
            if (log == null)
            {
                return ResponseBase.CommonError("成长档案不存在");
            }
            await _tryCalssApplyLogDAL.AddTryCalssApplyLog(new EtTryCalssApplyLog()
            {
                ApplyOt = DateTime.Now,
                ClassOt = null,
                ClassTime = string.Empty,
                CourseDesc = string.Empty,
                CourseId = null,
                HandleOt = null,
                HandleRemark = string.Empty,
                HandleStatus = EmTryCalssApplyHandleStatus.Unreviewed,
                HandleUser = null,
                IsDeleted = EmIsDeleted.Normal,
                Phone = request.Phone,
                RecommandStudentId = log.StudentId,
                SourceType = EmTryCalssSourceType.WeChat,
                StudentId = null,
                TenantId = request.TenantId,
                TouristName = request.Name,
                TouristRemark = request.Remark
            });

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(log.TenantId));
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CheckOnLogGet(CheckOnLogGetRequest request)
        {
            var log = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (log == null)
            {
                return ResponseBase.CommonError("考勤记录不存在");
            }
            return ResponseBase.Success(new CheckOnLogGetOutput()
            {
                CheckOt = log.CheckOt,
                CheckMediumUrl = AliyunOssUtil.GetAccessUrlHttps(log.CheckMedium),
                CheckType = log.CheckType,
                CheckTypeDesc = EmStudentCheckOnLogCheckType.GetStudentCheckOnLogCheckTypeDesc(log.CheckType)
            });
        }

        public async Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeGetRequest request)
        {
            var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.Id);
            if (accountLog == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            return ResponseBase.Success(new StudentAccountRechargeGetOutput()
            {
                BalanceGiveDesc = accountLog.BalanceGive.ToString("F2"),
                Id = accountLog.Id,
                BalanceRealDesc = accountLog.BalanceReal.ToString("F2"),
                BalanceSumDesc = accountLog.BalanceSum.ToString("F2"),
                Ot = accountLog.Ot,
                Phone = accountLog.Phone
            });
        }

        public async Task<ResponseBase> StudentAccountRechargeRuleGet(StudentAccountRechargeRuleGetRequest request)
        {
            var log = await this._appConfigDAL.GetAppConfig(EmAppConfigType.RechargeRuleConfig);
            if (log == null)
            {
                return ResponseBase.Success(new StudentAccountRechargeRuleGetOutput());
            }
            var rechargeRuleView = JsonConvert.DeserializeObject<StudentAccountRechargeRuleView>(log.ConfigValue);
            return ResponseBase.Success(new StudentAccountRechargeRuleGetOutput()
            {
                Explain = rechargeRuleView.Explain,
                ImgUrlKeyUrl = UrlHelper.GetUrl(rechargeRuleView.ImgUrlKey),
            });
        }

        public async Task<ResponseBase> StudentAccountRechargeLogGetPaging(StudentAccountRechargeLogGetPagingRequest request)
        {
            var pagingData = await _studentAccountRechargeLogDAL.GetPaging(request);
            var output = new List<StudentAccountRechargeLogGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new StudentAccountRechargeLogGetPagingOutput()
                {
                    CgNo = p.CgNo,
                    OtDesc = EtmsHelper.GetChangeLogTimeDesc(p.Ot),
                    StudentAccountRechargeId = p.StudentAccountRechargeId,
                    Type = p.Type,
                    TypeDesc = EmStudentAccountRechargeLogType.GetStudentAccountRechargeLogTypeDesc(p.Type),
                    CgBalanceRealDesc = EmStudentAccountRechargeLogType.GetValueDesc(p.CgBalanceReal, p.Type),
                    CgBalanceGiveDesc = EmStudentAccountRechargeLogType.GetValueDesc(p.CgBalanceGive, p.Type),
                    CgBalanceTotalDesc = EmStudentAccountRechargeLogType.GetValueDesc(p.CgBalanceTotal, p.Type),
                    ChangeType = EmStudentAccountRechargeLogType.GetValueChangeType(p.Type),
                    Id = p.Id
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentAccountRechargeLogGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
