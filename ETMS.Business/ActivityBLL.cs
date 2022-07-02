using ETMS.Business.Common;
using ETMS.Business.WxCore;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Activity.Output;
using ETMS.Entity.Dto.Activity.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.View.Activity;
using ETMS.Event.DataContract.Activity;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.Activity;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ActivityBLL : MiniProgramAccessBll, IActivityBLL
    {
        private readonly ISysActivityDAL _sysActivityDAL;

        private readonly IActivityMainDAL _activityMainDAL;

        private readonly IActivityRouteDAL _activityRouteDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly ISysActivityRouteItemDAL _sysActivityRouteItemDAL;
        public ActivityBLL(IAppConfigurtaionServices appConfigurtaionServices, ISysActivityDAL sysActivityDAL, IActivityMainDAL activityMainDAL, IActivityRouteDAL activityRouteDAL,
            ITenantConfigDAL tenantConfigDAL, IUserOperationLogDAL userOperationLogDAL, ISysTenantDAL sysTenantDAL,
            IEventPublisher eventPublisher, ISysActivityRouteItemDAL sysActivityRouteItemDAL)
            : base(appConfigurtaionServices)
        {
            this._sysActivityDAL = sysActivityDAL;
            this._activityMainDAL = activityMainDAL;
            this._activityRouteDAL = activityRouteDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._eventPublisher = eventPublisher;
            this._sysActivityRouteItemDAL = sysActivityRouteItemDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _activityMainDAL, _activityRouteDAL, _tenantConfigDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> ActivitySystemGetPaging(ActivitySystemGetPagingRequest request)
        {
            var pagingData = await _sysActivityDAL.GetPaging(request);
            var output = new List<ActivitySystemGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new ActivitySystemGetPagingOutput()
                {
                    ActivityType = p.ActivityType,
                    ActivityTypeDesc = p.ActivityTypeDesc,
                    CId = TenantLib.GetIdEncryptUrl(p.Id),
                    CourseName = p.CourseName,
                    CoverImage = p.CoverImage,
                    Name = p.Name,
                    Scenetype = p.Scenetype,
                    ScenetypeDesc = p.ScenetypeDesc,
                    StyleColumnColor = p.StyleColumnColor,
                    StyleType = p.StyleType,
                    Title = p.Title,
                    IsAllowPay = p.IsAllowPay,
                    ScenetypeStyleClass = p.ScenetypeStyleClass,
                    ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActivitySystemGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ActivityMainCreateInit(ActivityMainCreateInitRequest request)
        {
            var mySystemId = TenantLib.GetIdDecrypt(request.SystemId);
            var activityMain = await _sysActivityDAL.GetSysActivity(mySystemId);
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动模板不存在");
            }
            var myTenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var tenantInfo = myTenantConfig.TenantInfoConfig;
            var endDate = DateTime.Now.AddDays(10);
            var endTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 0, 0);
            switch (activityMain.ActivityType)
            {
                case EmActivityType.GroupPurchase:
                    var outputGroupPurchase = new ActivityMainCreateInitOfGroupPurchaseOutput()
                    {
                        ActivityExplan = activityMain.ActivityExplan,
                        CourseDesc = activityMain.CourseDesc,
                        CourseName = activityMain.CourseName,
                        StartTime = DateTime.Now.EtmsToMinuteString(),
                        EndTime = endTime.EtmsToMinuteString(),
                        GlobalOpenBullet = true,
                        GlobalPhone = tenantInfo.LinkPhone,
                        ImageCourse = EtmsHelper2.GetMediasUrl2(activityMain.ImageCourse),
                        ImageMains = EtmsHelper2.GetMediasUrl2(activityMain.ImageMain),
                        IsLimitStudent = false,
                        IsOpenCheckPhone = false,
                        IsAllowPay = activityMain.IsAllowPay,
                        IsOpenPay = true,
                        PayValue = 0,
                        PayType = EmActivityPayType.Type0,
                        MaxCount = activityMain.MaxCount,
                        Name = activityMain.Name,
                        OriginalPrice = activityMain.OriginalPrice,
                        SystemId = request.SystemId,
                        TenantLinkInfo = activityMain.TenantLinkInfo,
                        TenantLinkQRcode = null,
                        TenantIntroduceTxt = tenantInfo.Describe,
                        TenantIntroduceImg = new List<string>(),
                        TenantName = myTenant.Name,
                        Title = activityMain.Title,
                        GlobalOpenStatistics = true,
                        StyleType = activityMain.StyleType,
                        StyleBackColor = activityMain.StyleBackColor,
                        StyleColumnColor = activityMain.StyleColumnColor,
                        StyleColumnImg = activityMain.StyleColumnImg,
                        ActivityType = activityMain.ActivityType,
                        Scenetype = activityMain.Scenetype,
                        GroupPurchaseRuleInputs = new List<GroupPurchaseRuleInput>() {
                            new GroupPurchaseRuleInput(){
                                LimitCount = 2,
                                Money = 1688
                            }
                        }
                    };
                    return ResponseBase.Success(outputGroupPurchase);
                case EmActivityType.Haggling:
                    var outputHaggling = new ActivityMainCreateInitOfHaggleOutput()
                    {
                        ActivityExplan = activityMain.ActivityExplan,
                        CourseDesc = activityMain.CourseDesc,
                        CourseName = activityMain.CourseName,
                        StartTime = DateTime.Now.EtmsToMinuteString(),
                        EndTime = endTime.EtmsToMinuteString(),
                        GlobalOpenBullet = true,
                        GlobalPhone = tenantInfo.LinkPhone,
                        ImageCourse = EtmsHelper2.GetMediasUrl2(activityMain.ImageCourse),
                        ImageMains = EtmsHelper2.GetMediasUrl2(activityMain.ImageMain),
                        IsLimitStudent = false,
                        IsOpenCheckPhone = false,
                        IsAllowPay = activityMain.IsAllowPay,
                        IsOpenPay = true,
                        PayValue = 0,
                        PayType = EmActivityPayType.Type0,
                        MaxCount = activityMain.MaxCount,
                        Name = activityMain.Name,
                        OriginalPrice = activityMain.OriginalPrice,
                        SystemId = request.SystemId,
                        TenantLinkInfo = activityMain.TenantLinkInfo,
                        TenantLinkQRcode = null,
                        TenantIntroduceTxt = tenantInfo.Describe,
                        TenantIntroduceImg = new List<string>(),
                        TenantName = myTenant.Name,
                        Title = activityMain.Title,
                        GlobalOpenStatistics = true,
                        StyleType = activityMain.StyleType,
                        StyleBackColor = activityMain.StyleBackColor,
                        StyleColumnColor = activityMain.StyleColumnColor,
                        StyleColumnImg = activityMain.StyleColumnImg,
                        ActivityType = activityMain.ActivityType,
                        Scenetype = activityMain.Scenetype,
                        LowPrice = activityMain.OriginalPrice - 1,
                        LimitMustCount = 20,
                        MyRepeatHaggleHour = 2
                    };
                    return ResponseBase.Success(outputHaggling);
            }
            return ResponseBase.CommonError("模板无效");
        }

        private async Task<ResponseBase> ActivityMainOfGroupPurchaseSave(ActivityMainSaveOfGroupPurchaseRequest request,
            int activityStatus)
        {
            var activityMain = await _sysActivityDAL.GetSysActivity(TenantLib.GetIdDecrypt(request.SystemId));
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动模板不存在");
            }
            if (request.IsOpenPay)
            {
                var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
                if (myTenant.PayUnionType == EmPayUnionType.NotApplied)
                {
                    return ResponseBase.CommonError("请先绑定活动关联的支付账户，才能开启支付功能");
                }
            }
            var now = DateTime.Now;
            var ruleContent = new ActivityOfGroupPurchaseRuleContentView()
            {
                Item = request.GroupPurchaseRuleInputs.Select(p => new ActivityOfGroupPurchaseRuleItem()
                {
                    LimitCount = p.LimitCount,
                    Money = p.Money
                }).OrderBy(j => j.LimitCount).ToList()
            };
            var strRuleContent = Newtonsoft.Json.JsonConvert.SerializeObject(ruleContent);
            DateTime? publishTime = null;
            if (activityStatus == EmActivityStatus.Processing)
            {
                publishTime = now;
            }
            var entity = new EtActivityMain()
            {
                SystemActivityId = activityMain.Id,
                ActivityType = activityMain.ActivityType,
                StyleType = activityMain.StyleType,
                Scenetype = activityMain.Scenetype,
                StyleBackColor = activityMain.StyleBackColor,
                StyleColumnColor = activityMain.StyleColumnColor,
                StyleColumnImg = activityMain.StyleColumnImg,
                UVCount = 0,
                VisitCount = 0,
                IsDeleted = EmIsDeleted.Normal,
                FailCount = 0,
                FinishCount = 0,
                JoinCount = 0,
                PVCount = 0,
                RouteCount = 0,
                RuningCount = 0,
                TranspondCount = 0,
                IsAllowPay = activityMain.IsAllowPay,
                TenantId = request.LoginTenantId,
                ActivityStatus = activityStatus,
                MaxCount = request.MaxCount,
                CourseDesc = request.CourseDesc,
                ActivityExplan = request.ActivityExplan,
                CourseName = request.CourseName,
                ImageMain = EtmsHelper2.GetMediasKeys(request.ImageMains),
                CoverImage = request.ImageMains[0],
                CreateTime = now,
                StartTime = request.StartTime.Value,
                EndTime = request.EndTime.Value,
                EndTimeType = EmActivityMainEndTimeType.DateTime,
                EndValue = 0,
                Title = request.Title,
                TenantName = request.TenantName,
                Name = request.Name,
                PayType = request.PayType,
                PayValue = request.PayValue,
                IsOpenPay = request.IsOpenPay,
                StudentFieldName1 = request.StudentFieldName1,
                StudentFieldName2 = request.StudentFieldName2,
                GlobalOpenBullet = request.GlobalOpenBullet,
                GlobalPhone = request.GlobalPhone,
                IsOpenCheckPhone = request.IsOpenCheckPhone,
                OriginalPrice = request.OriginalPrice,
                StudentHisLimitType = request.IsLimitStudent ? EmBool.True : EmBool.False,
                TenantLinkInfo = request.TenantLinkInfo,
                TenantLinkQRcode = request.TenantLinkQRcode,
                TenantIntroduceTxt = request.TenantIntroduceTxt,
                TenantIntroduceImg = EtmsHelper2.GetMediasKeys(request.TenantIntroduceImg),
                ImageCourse = EtmsHelper2.GetMediasKeys(request.ImageCourse),
                RuleContent = strRuleContent,
                ShareQRCode = string.Empty,
                PublishTime = publishTime,
                ActivityTypeStyleClass = activityMain.ActivityTypeStyleClass,
                ScenetypeStyleClass = activityMain.ScenetypeStyleClass,
                GlobalOpenStatistics = request.GlobalOpenStatistics,
                IsShowInParent = true
            };
            await _activityMainDAL.AddActivityMain(entity);
            var key = $"qr_main_{request.LoginTenantId}_{entity.Id}.png";
            var scene = $"{request.LoginTenantId}_{entity.Id}";
            var mainShareQRCodeKey = await GenerateQrCode(request.LoginTenantId,
                AliyunOssFileTypeEnum.ActivityMainQrCode, key, MiniProgramPathConfig.ActivityMain, scene);
            await _activityMainDAL.UpdateActivityMainShareQRCode(entity.Id, mainShareQRCodeKey);

            await _userOperationLogDAL.AddUserLog(request, $"新建活动-{request.Title}", EmUserOperationType.Activity, now);

            var output = new ActivityMainOfGroupPurchaseSaveOutput()
            {
                MainShareQRCodeUrl = AliyunOssUtil.GetAccessUrlHttps(mainShareQRCodeKey),
                Name = entity.Name,
                CId = entity.Id
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActivityMainSaveOfGroupPurchase(ActivityMainSaveOfGroupPurchaseRequest request)
        {
            return await ActivityMainOfGroupPurchaseSave(request, EmActivityStatus.Unpublished);
        }

        public async Task<ResponseBase> ActivityMainSaveAndPublishOfGroupPurchase(ActivityMainSaveOfGroupPurchaseRequest request)
        {
            return await ActivityMainOfGroupPurchaseSave(request, EmActivityStatus.Processing);
        }

        private async Task<ResponseBase> ActivityMainOfHaggleSave(ActivityMainSaveOrPublishOfHaggleRequest request,
            int activityStatus)
        {
            var activityMain = await _sysActivityDAL.GetSysActivity(TenantLib.GetIdDecrypt(request.SystemId));
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动模板不存在");
            }
            if (request.IsOpenPay)
            {
                var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
                if (myTenant.PayUnionType == EmPayUnionType.NotApplied)
                {
                    return ResponseBase.CommonError("请先绑定活动关联的支付账户，才能开启支付功能");
                }
            }
            var now = DateTime.Now;
            DateTime? publishTime = null;
            if (activityStatus == EmActivityStatus.Processing)
            {
                publishTime = now;
            }
            var entity = new EtActivityMain()
            {
                SystemActivityId = activityMain.Id,
                ActivityType = activityMain.ActivityType,
                StyleType = activityMain.StyleType,
                Scenetype = activityMain.Scenetype,
                StyleBackColor = activityMain.StyleBackColor,
                StyleColumnColor = activityMain.StyleColumnColor,
                StyleColumnImg = activityMain.StyleColumnImg,
                UVCount = 0,
                VisitCount = 0,
                IsDeleted = EmIsDeleted.Normal,
                FailCount = 0,
                FinishCount = 0,
                JoinCount = 0,
                PVCount = 0,
                RouteCount = 0,
                RuningCount = 0,
                TranspondCount = 0,
                IsAllowPay = activityMain.IsAllowPay,
                TenantId = request.LoginTenantId,
                ActivityStatus = activityStatus,
                MaxCount = request.MaxCount,
                CourseDesc = request.CourseDesc,
                ActivityExplan = request.ActivityExplan,
                CourseName = request.CourseName,
                ImageMain = EtmsHelper2.GetMediasKeys(request.ImageMains),
                CoverImage = request.ImageMains[0],
                CreateTime = now,
                StartTime = request.StartTime.Value,
                EndTime = request.EndTime.Value,
                EndTimeType = EmActivityMainEndTimeType.DateTime,
                EndValue = 0,
                Title = request.Title,
                TenantName = request.TenantName,
                Name = request.Name,
                PayType = request.PayType,
                PayValue = request.PayValue,
                IsOpenPay = request.IsOpenPay,
                StudentFieldName1 = request.StudentFieldName1,
                StudentFieldName2 = request.StudentFieldName2,
                GlobalOpenBullet = request.GlobalOpenBullet,
                GlobalPhone = request.GlobalPhone,
                IsOpenCheckPhone = request.IsOpenCheckPhone,
                OriginalPrice = request.OriginalPrice,
                StudentHisLimitType = request.IsLimitStudent ? EmBool.True : EmBool.False,
                TenantLinkInfo = request.TenantLinkInfo,
                TenantLinkQRcode = request.TenantLinkQRcode,
                TenantIntroduceTxt = request.TenantIntroduceTxt,
                TenantIntroduceImg = EtmsHelper2.GetMediasKeys(request.TenantIntroduceImg),
                ImageCourse = EtmsHelper2.GetMediasKeys(request.ImageCourse),
                ShareQRCode = string.Empty,
                PublishTime = publishTime,
                ActivityTypeStyleClass = activityMain.ActivityTypeStyleClass,
                ScenetypeStyleClass = activityMain.ScenetypeStyleClass,
                GlobalOpenStatistics = request.GlobalOpenStatistics,
                IsShowInParent = true,
                RuleEx1 = request.LowPrice.ToString("F2"),
                RuleEx2 = request.LimitMustCount.ToString(),
                RuleEx3 = request.MyRepeatHaggleHour.ToString(),
                RuleContent = string.Empty
            };
            await _activityMainDAL.AddActivityMain(entity);
            var key = $"qr_main_{request.LoginTenantId}_{entity.Id}.png";
            var scene = $"{request.LoginTenantId}_{entity.Id}";
            var mainShareQRCodeKey = await GenerateQrCode(request.LoginTenantId,
                AliyunOssFileTypeEnum.ActivityMainQrCode, key, MiniProgramPathConfig.ActivityMain, scene);
            await _activityMainDAL.UpdateActivityMainShareQRCode(entity.Id, mainShareQRCodeKey);

            await _userOperationLogDAL.AddUserLog(request, $"新建活动-{request.Title}", EmUserOperationType.Activity, now);

            var output = new ActivityMainOfGroupPurchaseSaveOutput()
            {
                MainShareQRCodeUrl = AliyunOssUtil.GetAccessUrlHttps(mainShareQRCodeKey),
                Name = entity.Name,
                CId = entity.Id
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActivityMainSaveOfHaggle(ActivityMainSaveOrPublishOfHaggleRequest request)
        {
            return await ActivityMainOfHaggleSave(request, EmActivityStatus.Unpublished);
        }

        public async Task<ResponseBase> ActivityMainSaveAndPublishOfHaggle(ActivityMainSaveOrPublishOfHaggleRequest request)
        {
            return await ActivityMainOfHaggleSave(request, EmActivityStatus.Processing);
        }

        public async Task<ResponseBase> ActivityMainEdit(ActivityMainEditRequest request)
        {
            var activityMain = await _activityMainDAL.GetActivityMain(request.CId);
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            if (activityMain.StartTime >= request.EndTime)
            {
                return ResponseBase.CommonError("结束时间必须大于开始时间");
            }
            if (request.MaxCount < activityMain.MaxCount)
            {
                return ResponseBase.CommonError("限制数量必须大于原有设置");
            }

            activityMain.EndTime = request.EndTime;
            activityMain.Name = request.Name;
            activityMain.ImageMain = EtmsHelper2.GetMediasKeys(request.ImageMains);
            activityMain.CoverImage = request.ImageMains[0];
            activityMain.TenantName = request.TenantName;
            activityMain.Title = request.Title;
            activityMain.CourseName = request.CourseName;
            activityMain.CourseDesc = request.CourseDesc;
            activityMain.ImageCourse = EtmsHelper2.GetMediasKeys(request.ImageCourse);
            activityMain.OriginalPrice = request.OriginalPrice;
            activityMain.MaxCount = request.MaxCount;
            activityMain.StudentHisLimitType = request.IsLimitStudent ? EmBool.True : EmBool.False;
            activityMain.StudentFieldName1 = request.StudentFieldName1;
            activityMain.StudentFieldName2 = request.StudentFieldName2;
            activityMain.ActivityExplan = request.ActivityExplan;
            activityMain.TenantLinkInfo = request.TenantLinkInfo;
            activityMain.TenantLinkQRcode = request.TenantLinkQRcode;
            activityMain.TenantIntroduceTxt = request.TenantIntroduceTxt;
            activityMain.TenantIntroduceImg = EtmsHelper2.GetMediasKeys(request.TenantIntroduceImg);
            activityMain.GlobalPhone = request.GlobalPhone;
            activityMain.GlobalOpenBullet = request.GlobalOpenBullet;
            activityMain.IsOpenCheckPhone = request.IsOpenCheckPhone;
            activityMain.GlobalOpenStatistics = request.GlobalOpenStatistics;
            if (activityMain.ActivityType == EmActivityType.Haggling)
            {
                activityMain.RuleEx3 = request.MyRepeatHaggleHour.ToString();
            }
            await _activityMainDAL.EditActivityMain(activityMain);

            _eventPublisher.Publish(new SyncActivityBascInfoEvent(request.LoginTenantId)
            {
                NewActivityMain = activityMain
            });
            await _userOperationLogDAL.AddUserLog(request, $"编辑活动-{request.Title}", EmUserOperationType.Activity);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActivityMainGetPaging(ActivityMainGetPagingRequest request)
        {
            var pagingData = await _activityMainDAL.GetPaging(request);
            var output = new List<ActivityMainGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                var activityStatusResult = EmActivityStatus.GetActivityStatus(p.ActivityStatus, p.EndTime.Value);
                output.Add(new ActivityMainGetPagingOutput()
                {
                    CreateTime = p.CreateTime,
                    ActivityStatus = activityStatusResult.Item1,
                    ActivityStatusDesc = activityStatusResult.Item2,
                    EndTime = p.EndTime.Value,
                    ActivityType = p.ActivityType,
                    ActivityTypeDesc = EmActivityType.GetActivityTypeDesc(p.ActivityType),
                    CId = p.Id,
                    Name = p.Name,
                    PublishTime = p.PublishTime,
                    Scenetype = p.Scenetype,
                    ScenetypeDesc = EmActivityScenetype.GetActivityScenetypeDesc(p.Scenetype),
                    CoverImageUrl = p.CoverImage,
                    StartTime = p.StartTime,
                    SystemActivityId = p.SystemActivityId,
                    Title = p.Title,
                    ShareQRCode = AliyunOssUtil.GetAccessUrlHttps(p.ShareQRCode),
                    FailCount = p.FailCount,
                    FinishCount = p.FinishCount,
                    JoinCount = p.JoinCount,
                    PVCount = p.PVCount,
                    RouteCount = p.RouteCount,
                    RuningCount = p.RuningCount,
                    TranspondCount = p.TranspondCount,
                    UVCount = p.UVCount,
                    VisitCount = p.VisitCount,
                    StudentFieldName1 = p.StudentFieldName1,
                    StudentFieldName2 = p.StudentFieldName2,
                    ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                    ScenetypeStyleClass = p.ScenetypeStyleClass,
                    IsShowInParent = p.IsShowInParent
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActivityMainGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ActivityMainPublish(ActivityMainPublishRequest request)
        {
            var activityMain = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            if (activityMain.ActivityStatus == EmActivityStatus.Processing)
            {
                return ResponseBase.CommonError("活动已发布");
            }
            if (activityMain.EndTime <= DateTime.Now)
            {
                return ResponseBase.CommonError("活动已结束");
            }
            await _activityMainDAL.UpdateActivityMainStatus(request.ActivityMainId, EmActivityStatus.Processing);

            await _userOperationLogDAL.AddUserLog(request, $"发布活动-{activityMain.Title}", EmUserOperationType.Activity);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActivityMainOff(ActivityMainOffRequest request)
        {
            var activityMain = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            if (activityMain.ActivityStatus == EmActivityStatus.TakeDown)
            {
                return ResponseBase.CommonError("活动已下架");
            }
            if (activityMain.EndTime <= DateTime.Now)
            {
                return ResponseBase.CommonError("活动已结束");
            }
            await _activityMainDAL.UpdateActivityMainStatus(request.ActivityMainId, EmActivityStatus.TakeDown);

            await _userOperationLogDAL.AddUserLog(request, $"下架活动-{activityMain.Title}", EmUserOperationType.Activity);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActivityMainSetOn(ActivityMainSetOnRequest request)
        {
            var activityMain = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            if (activityMain.ActivityStatus == EmActivityStatus.Processing)
            {
                return ResponseBase.CommonError("活动已上架");
            }
            if (activityMain.EndTime <= DateTime.Now)
            {
                return ResponseBase.CommonError("活动已结束");
            }
            await _activityMainDAL.UpdateActivityMainStatus(request.ActivityMainId, EmActivityStatus.Processing);

            await _userOperationLogDAL.AddUserLog(request, $"上架活动-{activityMain.Title}", EmUserOperationType.Activity);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActivityMainSetShowInParent(ActivityMainSetShowInParentRequest request)
        {
            var activityMain = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            if (activityMain.IsShowInParent == request.NewIsShowInParent)
            {
                return ResponseBase.Success();
            }
            await _activityMainDAL.UpdateActivityMainIsShowInParent(request.ActivityMainId, request.NewIsShowInParent);

            var msg = string.Empty;
            if (request.NewIsShowInParent)
            {
                msg = "设置学员端可见";
            }
            else
            {
                msg = "设置学员端不可见";
            }
            await _userOperationLogDAL.AddUserLog(request, $"{msg}-{activityMain.Title}", EmUserOperationType.Activity);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActivityMainGetSimple(ActivityMainGetSimpleRequest request)
        {
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (p == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            var activityStatusResult = EmActivityStatus.GetActivityStatus(p.ActivityStatus, p.EndTime.Value);
            var output = new ActivityMainGetSimpleOutput()
            {
                CreateTime = p.CreateTime,
                ActivityStatus = activityStatusResult.Item1,
                ActivityStatusDesc = activityStatusResult.Item2,
                EndTime = p.EndTime.Value,
                ActivityType = p.ActivityType,
                ActivityTypeDesc = EmActivityType.GetActivityTypeDesc(p.ActivityType),
                CId = p.Id,
                Name = p.Name,
                PublishTime = p.PublishTime,
                Scenetype = p.Scenetype,
                ScenetypeDesc = EmActivityScenetype.GetActivityScenetypeDesc(p.Scenetype),
                CoverImageUrl = p.CoverImage,
                StartTime = p.StartTime,
                SystemActivityId = p.SystemActivityId,
                Title = p.Title,
                ShareQRCode = AliyunOssUtil.GetAccessUrlHttps(p.ShareQRCode),
                FailCount = p.FailCount,
                FinishCount = p.FinishCount,
                JoinCount = p.JoinCount,
                PVCount = p.PVCount,
                RouteCount = p.RouteCount,
                RuningCount = p.RuningCount,
                TranspondCount = p.TranspondCount,
                UVCount = p.UVCount,
                VisitCount = p.VisitCount,
                StudentFieldName1 = p.StudentFieldName1,
                StudentFieldName2 = p.StudentFieldName2,
                ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                ScenetypeStyleClass = p.ScenetypeStyleClass,
                IsOpenPay = p.IsOpenPay
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActivityMainDel(ActivityMainDelRequest request)
        {
            var activityMain = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            var isExistRoute = await _activityRouteDAL.ExistActivity(request.ActivityMainId);
            if (isExistRoute)
            {
                return ResponseBase.CommonError("活动进行中，无法删除");
            }
            await _activityMainDAL.DelActivityMain(request.ActivityMainId);
            await _sysActivityRouteItemDAL.DelSysActivityRouteItemByActivityId(request.LoginTenantId, request.ActivityMainId);

            await _userOperationLogDAL.AddUserLog(request, $"删除活动-{activityMain.Title}", EmUserOperationType.Activity);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActivityMainGetForEdit(ActivityMainGetForEditRequest request)
        {
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (p == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            var activityMain = await _sysActivityDAL.GetSysActivity(p.SystemActivityId);
            if (activityMain == null)
            {
                return ResponseBase.CommonError("活动模板不存在");
            }
            switch (p.ActivityType)
            {
                case EmActivityType.GroupPurchase:
                    var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(p.RuleContent);
                    var ruleOutput = ruleContent.Item.Select(p => new GroupPurchaseRuleInput()
                    {
                        LimitCount = p.LimitCount,
                        Money = p.Money
                    }).ToList();
                    var outputGroupPurchase = new ActivityMainCreateInitOfGroupPurchaseOutput()
                    {
                        ActivityExplan = p.ActivityExplan,
                        CourseDesc = p.CourseDesc,
                        CourseName = p.CourseName,
                        StartTime = p.StartTime.EtmsToMinuteString(),
                        EndTime = p.EndTime.Value.EtmsToMinuteString(),
                        GlobalOpenBullet = p.GlobalOpenBullet,
                        GlobalPhone = p.GlobalPhone,
                        GroupPurchaseRuleInputs = ruleOutput,
                        ImageCourse = EtmsHelper2.GetMediasUrl2(p.ImageCourse),
                        ImageMains = EtmsHelper2.GetMediasUrl2(p.ImageMain),
                        IsAllowPay = p.IsAllowPay,
                        IsLimitStudent = p.StudentHisLimitType == EmBool.True,
                        IsOpenCheckPhone = p.IsOpenCheckPhone,
                        IsOpenPay = p.IsOpenPay,
                        MaxCount = p.MaxCount,
                        Name = p.Name,
                        OriginalPrice = p.OriginalPrice,
                        PayType = p.PayType,
                        PayValue = p.PayValue,
                        StudentFieldName1 = p.StudentFieldName1,
                        StudentFieldName2 = p.StudentFieldName2,
                        TenantIntroduceImg = EtmsHelper2.GetMediasUrl2(p.TenantIntroduceImg),
                        TenantIntroduceTxt = p.TenantIntroduceTxt,
                        TenantLinkInfo = p.TenantLinkInfo,
                        TenantLinkQRcode = p.TenantLinkQRcode,
                        TenantName = p.TenantName,
                        Title = p.Title,
                        GlobalOpenStatistics = p.GlobalOpenStatistics,
                        IsOpenStudentFieldName1 = !string.IsNullOrEmpty(p.StudentFieldName1),
                        IsOpenStudentFieldName2 = !string.IsNullOrEmpty(p.StudentFieldName2),
                        StyleBackColor = activityMain.StyleBackColor,
                        StyleColumnColor = activityMain.StyleColumnColor,
                        StyleColumnImg = activityMain.StyleColumnImg,
                        StyleType = activityMain.StyleType,
                        ActivityType = p.ActivityType,
                        Scenetype = p.Scenetype,
                        SystemId = string.Empty
                    };
                    return ResponseBase.Success(outputGroupPurchase);
                case EmActivityType.Haggling:
                    var outputHaggling = new ActivityMainCreateInitOfHaggleOutput()
                    {
                        ActivityExplan = p.ActivityExplan,
                        CourseDesc = p.CourseDesc,
                        CourseName = p.CourseName,
                        StartTime = p.StartTime.EtmsToMinuteString(),
                        EndTime = p.EndTime.Value.EtmsToMinuteString(),
                        GlobalOpenBullet = p.GlobalOpenBullet,
                        GlobalPhone = p.GlobalPhone,
                        ImageCourse = EtmsHelper2.GetMediasUrl2(p.ImageCourse),
                        ImageMains = EtmsHelper2.GetMediasUrl2(p.ImageMain),
                        IsAllowPay = p.IsAllowPay,
                        IsLimitStudent = p.StudentHisLimitType == EmBool.True,
                        IsOpenCheckPhone = p.IsOpenCheckPhone,
                        IsOpenPay = p.IsOpenPay,
                        MaxCount = p.MaxCount,
                        Name = p.Name,
                        OriginalPrice = p.OriginalPrice,
                        PayType = p.PayType,
                        PayValue = p.PayValue,
                        StudentFieldName1 = p.StudentFieldName1,
                        StudentFieldName2 = p.StudentFieldName2,
                        TenantIntroduceImg = EtmsHelper2.GetMediasUrl2(p.TenantIntroduceImg),
                        TenantIntroduceTxt = p.TenantIntroduceTxt,
                        TenantLinkInfo = p.TenantLinkInfo,
                        TenantLinkQRcode = p.TenantLinkQRcode,
                        TenantName = p.TenantName,
                        Title = p.Title,
                        GlobalOpenStatistics = p.GlobalOpenStatistics,
                        IsOpenStudentFieldName1 = !string.IsNullOrEmpty(p.StudentFieldName1),
                        IsOpenStudentFieldName2 = !string.IsNullOrEmpty(p.StudentFieldName2),
                        StyleBackColor = activityMain.StyleBackColor,
                        StyleColumnColor = activityMain.StyleColumnColor,
                        StyleColumnImg = activityMain.StyleColumnImg,
                        StyleType = activityMain.StyleType,
                        ActivityType = p.ActivityType,
                        Scenetype = p.Scenetype,
                        LowPrice = p.RuleEx1.ToDecimal(),
                        LimitMustCount = p.RuleEx2.ToInt(),
                        MyRepeatHaggleHour = p.RuleEx3.ToInt(),
                        SystemId = string.Empty
                    };
                    return ResponseBase.Success(outputHaggling);
            }

            return ResponseBase.CommonError("活动模板错误");
        }

        public async Task<ResponseBase> ActivityRouteGetPaging(ActivityRouteGetPagingRequest request)
        {
            var myActivity = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (myActivity == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            var minCount = 0;
            var maxCount = 0;
            if (myActivity.ActivityType == EmActivityType.GroupPurchase)
            {
                var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(myActivity.RuleContent);
                minCount = ruleContent.Item.First().LimitCount;
                maxCount = ruleContent.Item.Last().LimitCount;
            }
            var pagingData = await _activityRouteDAL.GetPagingRoute(request);
            var output = new List<ActivityRouteGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                var status = 0;
                var statusDesc = string.Empty;
                if (p.CountFinish >= p.CountLimit)
                {
                    status = 1;
                    statusDesc = "已完成";
                }
                else
                {
                    statusDesc = "未完成";
                }
                var payStatusDesc = string.Empty;
                if (p.IsNeedPay)
                {
                    payStatusDesc = EmActivityRoutePayStatus.GetActivityRoutePayStatusDesc(p.PayStatus);
                }
                var groupPurchaseFinishDesc = string.Empty;
                if (myActivity.ActivityType == EmActivityType.GroupPurchase)
                {
                    groupPurchaseFinishDesc = ComBusiness5.GetSysActivityGroupPurchaseStatusDesc(minCount, maxCount, p.CountFinish);
                }
                output.Add(new ActivityRouteGetPagingOutput()
                {
                    ActivityRouteId = p.Id,
                    ActivityId = p.ActivityId,
                    AvatarUrl = p.AvatarUrl,
                    CountFinish = p.CountFinish,
                    CountLimit = p.CountLimit,
                    CreateTime = p.CreateTime,
                    NickName = p.NickName,
                    PayFinishTime = p.PayFinishTime,
                    PaySum = p.PaySum,
                    ShareQRCode = AliyunOssUtil.GetAccessUrlHttps(p.ShareQRCode),
                    StudentFieldValue1 = p.StudentFieldValue1,
                    StudentFieldValue2 = p.StudentFieldValue2,
                    StudentName = p.StudentName,
                    StudentPhone = p.StudentPhone,
                    Status = status,
                    StatusDesc = statusDesc,
                    Tag = p.Tag,
                    ActivityType = p.ActivityType,
                    PayStatus = p.PayStatus,
                    PayStatusDesc = payStatusDesc,
                    GroupPurchaseFinishDesc = groupPurchaseFinishDesc,
                    PayOrderNo = p.PayOrderNo
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActivityRouteGetPagingOutput>(pagingData.Item2, output));
        }

        [Obsolete("使用ActivityRouteItemGetPaging")]
        public async Task<ResponseBase> ActivityRouteItemGet(ActivityRouteItemGetRequest request)
        {
            var bucket = await _activityRouteDAL.GetActivityRouteBucket(request.ActivityRouteId);
            if (bucket == null)
            {
                return ResponseBase.CommonError("开团记录不存在");
            }
            var output = new List<ActivityRouteItemGetOutput>();
            var activityRouteItems = bucket.ActivityRouteItems;
            if (activityRouteItems == null || activityRouteItems.Count == 0)
            {
                return ResponseBase.Success(output);
            }
            foreach (var p in activityRouteItems)
            {
                output.Add(new ActivityRouteItemGetOutput()
                {
                    ItemId = p.Id,
                    ActivityId = p.ActivityId,
                    ActivityRouteId = p.ActivityRouteId,
                    MiniPgmUserId = p.MiniPgmUserId,
                    AvatarUrl = p.AvatarUrl,
                    CreateTime = p.CreateTime,
                    NickName = p.NickName,
                    PayFinishTime = p.PayFinishTime,
                    PaySum = p.PaySum,
                    StudentFieldValue1 = p.StudentFieldValue1,
                    StudentFieldValue2 = p.StudentFieldValue2,
                    StudentName = p.StudentName,
                    StudentPhone = p.StudentPhone,
                    Tag = p.Tag,
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActivityRouteItemGetPaging(ActivityRouteItemGetPagingRequest request)
        {
            var pagingData = await _activityRouteDAL.GetPagingRouteItem(request);
            var output = new List<ActivityRouteItemGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                var payStatusDesc = string.Empty;
                if (p.IsNeedPay)
                {
                    payStatusDesc = EmActivityRoutePayStatus.GetActivityRoutePayStatusDesc(p.PayStatus);
                }
                output.Add(new ActivityRouteItemGetPagingOutput()
                {
                    ItemId = p.Id,
                    ActivityId = p.ActivityId,
                    ActivityRouteId = p.ActivityRouteId,
                    MiniPgmUserId = p.MiniPgmUserId,
                    AvatarUrl = p.AvatarUrl,
                    CreateTime = p.CreateTime,
                    NickName = p.NickName,
                    PayFinishTime = p.PayFinishTime,
                    PaySum = p.PaySum,
                    StudentFieldValue1 = p.StudentFieldValue1,
                    StudentFieldValue2 = p.StudentFieldValue2,
                    StudentName = p.StudentName,
                    StudentPhone = p.StudentPhone,
                    Tag = p.Tag,
                    ActivityType = p.ActivityType,
                    PayStatus = p.PayStatus,
                    PayStatusDesc = payStatusDesc,
                    PayOrderNo = p.PayOrderNo
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActivityRouteItemGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ActivityRouteSetTag(ActivityRouteSetTagRequest request)
        {
            await _activityRouteDAL.UpdateActivityRouteTag(request.RouteId, request.Tag);

            await _userOperationLogDAL.AddUserLog(request, $"标记活动参与详情：{request.Tag}", EmUserOperationType.Activity);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActivityRouteItemSetTag(ActivityRouteItemSetTagRequest request)
        {
            await _activityRouteDAL.UpdateActivityRouteItemTag(request.RouteItemId, request.Tag);

            await _userOperationLogDAL.AddUserLog(request, $"标记活动参与详情：{request.Tag}", EmUserOperationType.Activity);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActivityHaggleLogGet(ActivityHaggleLogGetRequest request)
        {
            var bucket = await _activityRouteDAL.GetActivityRouteBucket(request.ActivityRouteId);
            if (bucket == null)
            {
                return ResponseBase.CommonError("开团记录不存在");
            }
            var output = new List<ActivityHaggleLogGetOutput>();
            var activityHaggleLogs = bucket.ActivityHaggleLogs;
            if (activityHaggleLogs == null || activityHaggleLogs.Count == 0)
            {
                return ResponseBase.Success(output);
            }
            foreach (var p in activityHaggleLogs)
            {
                output.Add(new ActivityHaggleLogGetOutput()
                {
                    ActivityId = p.ActivityId,
                    ActivityRouteId = p.ActivityRouteId,
                    AvatarUrl = p.AvatarUrl,
                    CreateDate = p.CreateDate,
                    CreateTime = p.CreateTime,
                    ItemId = p.Id,
                    NickName = p.NickName
                });
            }
            return ResponseBase.Success(output);
        }
    }
}
