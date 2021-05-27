using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IBusiness.SysOp;
using ETMS.Business.Common;

namespace ETMS.Business
{
    public class AppConfigBLL : IAppConfigBLL
    {
        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly ISysSafeSmsCodeCheckBLL _sysSafeSmsCodeCheckBLL;

        public AppConfigBLL(ITenantConfigDAL tenantConfigDAL, IUserOperationLogDAL userOperationLogDAL, ISysTenantDAL sysTenantDAL, IHttpContextAccessor httpContextAccessor,
            IAppConfigurtaionServices appConfigurtaionServices, ISysSafeSmsCodeCheckBLL sysSafeSmsCodeCheckBLL)
        {
            this._tenantConfigDAL = tenantConfigDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._sysSafeSmsCodeCheckBLL = sysSafeSmsCodeCheckBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _tenantConfigDAL, _userOperationLogDAL);
        }

        public async Task<TenantConfig> TenantConfigGet(int tenantId)
        {
            return await _tenantConfigDAL.GetTenantConfig();
        }

        public async Task<ResponseBase> TenantConfigGet(TenantConfigGetRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            var output = new TenantConfigGetOutput()
            {
                ClassCheckSignConfig = config.ClassCheckSignConfig,
                ParentSetConfig = config.ParentSetConfig,
                PrintConfig = config.PrintConfig,
                StudentCourseRenewalConfig = config.StudentCourseRenewalConfig,
                StudentNoticeConfig = config.StudentNoticeConfig,
                UserNoticeConfig = config.UserNoticeConfig,
                TenantInfoConfig = config.TenantInfoConfig,
                TeacherSetConfig = config.TeacherSetConfig,
                StudentCheckInConfig = config.StudentCheckInConfig,
                StudentRecommendConfig = config.StudentRecommendConfig,
                TenantOtherConfig = config.TenantOtherConfig,
                OtherOutput = new OtherOutput()
            };
            output.OtherOutput.StartClassDayBeforeTimeValueDesc = EtmsHelper.GetTimeDesc(output.StudentNoticeConfig.StartClassDayBeforeTimeValue);
            if (!string.IsNullOrEmpty(config.ParentSetConfig.LoginImage))
            {
                output.OtherOutput.ParentLoginImageUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, config.ParentSetConfig.LoginImage);
            }
            if (!string.IsNullOrEmpty(config.TeacherSetConfig.LoginImage))
            {
                output.OtherOutput.TeacherLoginImageUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, config.TeacherSetConfig.LoginImage);
            }
            if (!string.IsNullOrEmpty(config.StudentRecommendConfig.RecommendDesImg))
            {
                output.OtherOutput.RecommendDesImgUrl = UrlHelper.GetUrl(config.StudentRecommendConfig.RecommendDesImg);
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ClassCheckSignConfigSave(ClassCheckSignConfigSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.ClassCheckSignConfig.MakeupIsDeClassTimes = request.MakeupIsDeClassTimes;
            config.ClassCheckSignConfig.RewardPointsMustApply = request.RewardPointsMustApply;
            config.ClassCheckSignConfig.DayCourseMustSetStartEndTime = request.DayCourseMustSetStartEndTime;
            config.ClassCheckSignConfig.MustBuyCourse = request.MustBuyCourse;
            config.ClassCheckSignConfig.MustEnoughSurplusClassTimes = request.MustEnoughSurplusClassTimes;
            config.ClassCheckSignConfig.TryCalssNoticeTrackUser = request.TryCalssNoticeTrackUser;
            config.ClassCheckSignConfig.IsCanDeDecimal = request.IsCanDeDecimal;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "点名设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentNoticeConfigSave(StudentNoticeConfigSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.StudentNoticeConfig.StartClassWeChat = request.StartClassWeChat;
            config.StudentNoticeConfig.StartClassSms = request.StartClassSms;
            config.StudentNoticeConfig.ClassCheckSignWeChat = request.ClassCheckSignWeChat;
            config.StudentNoticeConfig.ClassCheckSignSms = request.ClassCheckSignSms;
            config.StudentNoticeConfig.OrderByWeChat = request.OrderByWeChat;
            config.StudentNoticeConfig.OrderBySms = request.OrderBySms;
            config.StudentNoticeConfig.TeacherClassEvaluateWeChat = request.TeacherClassEvaluateWeChat;
            config.StudentNoticeConfig.StudentGrowUpRecordWeChat = request.StudentGrowUpRecordWeChat;
            config.StudentNoticeConfig.StudentAskForLeaveCheckSms = request.StudentAskForLeaveCheckSms;
            config.StudentNoticeConfig.StudentAskForLeaveCheckWeChat = request.StudentAskForLeaveCheckWeChat;
            config.StudentNoticeConfig.StudentHomeworkWeChat = request.StudentHomeworkWeChat;
            config.StudentNoticeConfig.StudentHomeworkCommentWeChat = request.StudentHomeworkCommentWeChat;
            config.StudentNoticeConfig.ClassRecordStudentChangeWeChat = request.ClassRecordStudentChangeWeChat;
            config.StudentNoticeConfig.StudentCourseNotEnoughWeChat = request.StudentCourseNotEnoughWeChat;
            config.StudentNoticeConfig.StudentCourseNotEnoughSms = request.StudentCourseNotEnoughSms;
            config.StudentNoticeConfig.StudentCheckOnWeChat = request.StudentCheckOnWeChat;
            config.StudentNoticeConfig.StudentCheckOnSms = request.StudentCheckOnSms;
            config.StudentNoticeConfig.StudentCouponsWeChat = request.StudentCouponsWeChat;
            config.StudentNoticeConfig.StudentAccountRechargeChangedWeChat = request.StudentAccountRechargeChangedWeChat;
            config.StudentNoticeConfig.StudentAccountRechargeChangedSms = request.StudentAccountRechargeChangedSms;
            config.StudentNoticeConfig.StudentCourseSurplusChangedWeChat = request.StudentCourseSurplusChangedWeChat;
            config.StudentNoticeConfig.StudentCourseSurplusChangedSms = request.StudentCourseSurplusChangedSms;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "通知设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StartClassNoticeSave(StartClassNoticeSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.StudentNoticeConfig.StartClassDayBeforeIsOpen = request.StartClassDayBeforeIsOpen;
            config.StudentNoticeConfig.StartClassDayBeforeTimeValue = request.StartClassDayBeforeTimeValue;
            config.StudentNoticeConfig.StartClassBeforeMinuteIsOpen = request.StartClassBeforeMinuteIsOpen;
            config.StudentNoticeConfig.StartClassBeforeMinuteValue = request.StartClassBeforeMinuteValue;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "通知设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserNoticeConfigSave(UserNoticeConfigSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.UserNoticeConfig.StartClassWeChat = request.StartClassWeChat;
            config.UserNoticeConfig.StartClassSms = request.StartClassSms;
            config.UserNoticeConfig.StudentHomeworkSubmitWeChat = request.StudentHomeworkSubmitWeChat;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "通知设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserStartClassNoticeSave(UserStartClassNoticeSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.UserNoticeConfig.StartClassBeforeMinuteValue = request.StartClassBeforeMinuteValue;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "通知设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserWeChatNoticeRemarkSave(UserWeChatNoticeRemarkSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.UserNoticeConfig.WeChatNoticeRemark = request.WeChatNoticeRemark;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "通知设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WeChatNoticeRemarkSave(WeChatNoticeRemarkSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.StudentNoticeConfig.WeChatNoticeRemark = request.WeChatNoticeRemark;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "通知设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseRenewalConfigSave(StudentCourseRenewalConfigSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.StudentCourseRenewalConfig.LimitClassTimes = request.LimitClassTimes;
            config.StudentCourseRenewalConfig.LimitDay = request.LimitDay;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "机构信息设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> PrintConfigGet(PrintConfigGetRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            var printConfig = config.PrintConfig;
            if (string.IsNullOrEmpty(printConfig.Name))
            {
                var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
                printConfig.Name = myTenant.Name;
            }
            return ResponseBase.Success(new PrintConfigGetOutput()
            {
                BottomDesc = printConfig.BottomDesc,
                Name = printConfig.Name,
                PrintType = printConfig.PrintType,
                TagImgKey = printConfig.TagImgKey,
                TagImg = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, printConfig.TagImgKey)
            });
        }

        public async Task<ResponseBase> PrintConfigSave(PrintConfigSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.PrintConfig.Name = request.Name;
            config.PrintConfig.BottomDesc = request.BottomDesc;
            config.PrintConfig.PrintType = request.PrintType;
            config.PrintConfig.TagImgKey = request.TagImgKey;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "收据打印设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ParentBannerGet(ParentBannerGetRequest request)
        {
            var output = new List<ParentBannerGetOutput>();
            var config = await _tenantConfigDAL.GetTenantConfig();
            foreach (var p in config.ParentSetConfig.ParentBanners)
            {
                if (string.IsNullOrEmpty(p.ImgKey))
                {
                    continue;
                }
                output.Add(new ParentBannerGetOutput()
                {
                    ImgKey = p.ImgKey,
                    ImgUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p.ImgKey),
                    UrlKey = p.UrlKey
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ParentBannerSave(ParentBannerSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            var banners = new List<ParentBanner>();
            if (request.ParentBannerSets != null && request.ParentBannerSets.Any())
            {
                foreach (var p in request.ParentBannerSets)
                {
                    banners.Add(new ParentBanner
                    {
                        ImgKey = p.ImgKey,
                        UrlKey = p.UrlKey
                    });
                }
            }
            config.ParentSetConfig.ParentBanners = banners;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "家长端设置-首页banner图设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantShowSave(TenantShowSaveRequest request)
        {
            var chekSmsResult = _sysSafeSmsCodeCheckBLL.SysSafeSmsCodeCheck(request.LoginTenantId, request.SmsCode);
            if (!chekSmsResult.IsResponseSuccess())
            {
                return chekSmsResult;
            }
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.ParentSetConfig.LoginImage = request.ParentLoginImage;
            config.ParentSetConfig.Title = request.ParentTitle;
            config.TeacherSetConfig.LoginImage = request.TeacherLoginImage;
            config.TeacherSetConfig.Title = request.TeacherTitle;
            config.TenantInfoConfig.Address = request.TenantAddress;
            config.TenantInfoConfig.Describe = request.TenantDescribe;
            config.TenantInfoConfig.LinkName = request.TenantLinkName;
            config.TenantInfoConfig.LinkPhone = request.TenantLinkPhone;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "机构展示设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseNotEnoughCountSave(StudentCourseNotEnoughCountSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.StudentNoticeConfig.StudentCourseNotEnoughCount = request.StudentCourseNotEnoughCount;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "通知设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> GetTenantInfoH5(int tenantId)
        {
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            _tenantConfigDAL.InitTenantId(tenantId);
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var teacherLoginImage = string.Empty;
            if (!string.IsNullOrEmpty(tenantConfig.TeacherSetConfig.LoginImage))
            {
                teacherLoginImage = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, tenantConfig.TeacherSetConfig.LoginImage);
            }
            return ResponseBase.Success(new GetTenantInfoH5Output()
            {
                TenantAddress = tenantConfig.TenantInfoConfig.Address,
                TenantDescribe = tenantConfig.TenantInfoConfig.Describe,
                TenantLinkName = tenantConfig.TenantInfoConfig.LinkName,
                TenantLinkPhone = tenantConfig.TenantInfoConfig.LinkPhone,
                TenantName = myTenant.Name,
                TenantNickName = myTenant.SmsSignature,
                TeacherHtmlTitle = tenantConfig.TeacherSetConfig.Title,
                TeacherLoginImage = teacherLoginImage
            });
        }

        public async Task<ResponseBase> GetTenantInfoH5ByNo(GetTenantInfoH5ByNoRequest request)
        {
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            if (tenantId == 0)
            {
                return ResponseBase.Success(new GetTenantInfoH5Output());
            }
            return await GetTenantInfoH5(tenantId);
        }

        public async Task<ResponseBase> GetTenantInfoH5(GetTenantInfoH5Request request)
        {
            return await GetTenantInfoH5(request.LoginTenantId);
        }

        public async Task<ResponseBase> StudentCheckInConfigSave(StudentCheckInConfigSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.StudentCheckInConfig.StudentUseCardCheckIn.IntervalTimeCard = request.IntervalTimeCard;
            config.StudentCheckInConfig.StudentUseCardCheckIn.IsMustCheckOutCard = request.IsMustCheckOutCard;
            config.StudentCheckInConfig.StudentUseCardCheckIn.IsRelationClassTimesCard = request.IsRelationClassTimesCard;
            config.StudentCheckInConfig.StudentUseCardCheckIn.RelationClassTimesLimitMinuteCard = request.RelationClassTimesLimitMinuteCard;
            config.StudentCheckInConfig.StudentUseCardCheckIn.IsShowQuickCardCheck = request.IsShowQuickCardCheck;

            config.StudentCheckInConfig.StudentUseFaceCheckIn.IntervalTimeFace = request.IntervalTimeFace;
            config.StudentCheckInConfig.StudentUseFaceCheckIn.IsMustCheckOutFace = request.IsMustCheckOutFace;
            config.StudentCheckInConfig.StudentUseFaceCheckIn.IsRelationClassTimesFace = request.IsRelationClassTimesFace;
            config.StudentCheckInConfig.StudentUseFaceCheckIn.RelationClassTimesLimitMinuteFace = request.RelationClassTimesLimitMinuteFace;

            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "考勤设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentRecommendConfigSave(StudentRecommendConfigSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.StudentRecommendConfig.IsOpenRegistered = request.IsOpenRegistered;
            config.StudentRecommendConfig.RegisteredGivePoints = request.RegisteredGivePoints;
            config.StudentRecommendConfig.RegisteredGiveMoney = request.RegisteredGiveMoney;
            config.StudentRecommendConfig.IsOpenBuy = request.IsOpenBuy;
            config.StudentRecommendConfig.BuyGivePoints = request.BuyGivePoints;
            config.StudentRecommendConfig.BuyGiveMoney = request.BuyGiveMoney;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "学员推荐有奖设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentRecommendConfigSave2(StudentRecommendConfigSave2Request request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.StudentRecommendConfig.RecommendDesText = request.RecommendDesText;
            config.StudentRecommendConfig.RecommendDesImg = request.RecommendDesImg;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "学员推荐有奖设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantOtherConfigSave(TenantOtherConfigSaveRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            config.TenantOtherConfig.ValidPhoneType = request.ValidPhoneType;
            await _tenantConfigDAL.SaveTenantConfig(config);
            await _userOperationLogDAL.AddUserLog(request, "机构设置", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }
    }
}
