using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Business.Common;
using ETMS.Business.WxCore;
using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.OpenParent2.Output;
using ETMS.Entity.Dto.OpenParent2.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.View.Activity;
using ETMS.Event.DataContract.Activity;
using ETMS.IBusiness.Open;
using ETMS.IDataAccess;
using ETMS.IDataAccess.Activity;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Pay.Suixing;
using ETMS.Pay.Suixing.Utility.ExternalDto.Request;
using ETMS.Utility;
using Newtonsoft.Json;

namespace ETMS.Business.Open
{
    public class OpenParent2BLL : MiniProgramAccessBll, IOpenParent2BLL
    {
        private readonly ISysWechatMiniPgmUserDAL _sysWechatMiniPgmUserDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IActivityMainDAL _activityMainDAL;

        private readonly IActivityRouteDAL _activityRouteDAL;

        private readonly IActivityVisitorDAL _activityVisitorDAL;

        private readonly ISysActivityRouteItemDAL _sysActivityRouteItemDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IDistributedLockDAL _distributedLockDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IPaySuixingService _paySuixingService;

        private readonly ISysTenantSuixingAccountDAL _sysTenantSuixingAccountDAL;

        private readonly ITenantConfig2DAL _tenantConfig2DAL;

        public OpenParent2BLL(IAppConfigurtaionServices appConfigurtaionServices, ISysWechatMiniPgmUserDAL sysWechatMiniPgmUserDAL, ISysTenantDAL sysTenantDAL,
           IActivityMainDAL activityMainDAL, IActivityRouteDAL activityRouteDAL, IActivityVisitorDAL activityVisitorDAL,
           ISysActivityRouteItemDAL sysActivityRouteItemDAL, IEventPublisher eventPublisher,
           IDistributedLockDAL distributedLockDAL, IStudentDAL studentDAL, IPaySuixingService paySuixingService,
           ISysTenantSuixingAccountDAL sysTenantSuixingAccountDAL, ITenantConfig2DAL tenantConfig2DAL)
            : base(appConfigurtaionServices)
        {
            this._sysWechatMiniPgmUserDAL = sysWechatMiniPgmUserDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._activityMainDAL = activityMainDAL;
            this._activityRouteDAL = activityRouteDAL;
            this._activityVisitorDAL = activityVisitorDAL;
            this._sysActivityRouteItemDAL = sysActivityRouteItemDAL;
            this._eventPublisher = eventPublisher;
            this._distributedLockDAL = distributedLockDAL;
            this._studentDAL = studentDAL;
            this._paySuixingService = paySuixingService;
            this._sysTenantSuixingAccountDAL = sysTenantSuixingAccountDAL;
            this._tenantConfig2DAL = tenantConfig2DAL;
        }

        private void InitTenantId(int tenantId)
        {
            _activityMainDAL.InitTenantId(tenantId);
            _activityRouteDAL.InitTenantId(tenantId);
            _activityVisitorDAL.InitTenantId(tenantId);
            _studentDAL.InitTenantId(tenantId);
            _tenantConfig2DAL.InitTenantId(tenantId);
        }

        public async Task<ResponseBase> WxMiniLogin(WxMiniLoginRequest request)
        {
            var loginResult = base.WxLogin(request.Code);
            if (loginResult == null)
            {
                return ResponseBase.CommonError("微信登录失败");
            }
            var log = await _sysWechatMiniPgmUserDAL.GetWechatMiniPgmUser(loginResult.openid);
            if (log == null)
            {
                log = new SysWechatMiniPgmUser()
                {
                    AvatarUrl = string.Empty,
                    CreateTime = DateTime.Now,
                    IsDeleted = EmIsDeleted.Normal,
                    NickName = string.Empty,
                    Phone = string.Empty,
                    Remark = string.Empty,
                    TenantId = null,
                    UpdateTime = null,
                    OpenId = loginResult.openid,
                    Unionid = loginResult.unionid,
                    SessionKey = loginResult.session_key
                };
                await _sysWechatMiniPgmUserDAL.AddWechatMiniPgmUser(log);
            }
            else
            {
                log.SessionKey = loginResult.session_key;
                await _sysWechatMiniPgmUserDAL.EditWechatMiniPgmUser(log);
            }
            var miniPgmUserId = log.Id;
            var strSignature = ParentSignatureLib.GetOpenParent2Signature(miniPgmUserId);
            return ResponseBase.Success(new WxMiniLoginOutput()
            {
                S = strSignature,
                U = miniPgmUserId,
                OpenId = loginResult.openid,
                Unionid = loginResult.unionid,
                NickName = log.NickName,
                AvatarUrl = log.AvatarUrl,
            });
        }

        public async Task<ResponseBase> WxMiniEditUserInfo(WxMiniEditUserInfoRequest request)
        {
            var log = await _sysWechatMiniPgmUserDAL.GetWechatMiniPgmUser(request.MiniPgmUserId);
            if (log == null)
            {
                LOG.Log.Error("[WxMiniEditUserInfo]用户未注册", request, this.GetType());
                return ResponseBase.CommonError("用户未注册");
            }
            log.NickName = request.NickName;
            log.AvatarUrl = request.AvatarUrl;
            log.UpdateTime = DateTime.Now;
            await _sysWechatMiniPgmUserDAL.EditWechatMiniPgmUser(log);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> DecodedPhoneNumber(DecodedPhoneNumberRequest request)
        {
            var log = await _sysWechatMiniPgmUserDAL.GetWechatMiniPgmUser(request.MiniPgmUserId);
            if (log == null)
            {
                LOG.Log.Error("[DecodedPhoneNumber]用户未注册", request, this.GetType());
                return ResponseBase.CommonError("用户未注册");
            }
            var result = base.WxDecodedPhoneNumber(log.SessionKey, request.EncryptedData, request.Iv);
            if (result == null)
            {
                return ResponseBase.CommonError("获取手机号码失败");
            }
            var output = new DecodedPhoneNumberPhone()
            {
                Phone = result.phoneNumber
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> WxMiniActivityRouteItemGetPaging(WxMiniActivityRouteItemGetPagingRequest request)
        {
            var pagingData = await _sysActivityRouteItemDAL.GetPagingRouteItem(request);
            var output = new List<WxMiniActivityRouteItemGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new WxMiniActivityRouteItemGetPagingOutput()
                    {
                        ActivityCoverImage = p.ActivityCoverImage,
                        ActivityEndTime = p.ActivityEndTime,
                        ActivityId = p.ActivityId,
                        ActivityName = p.ActivityName,
                        ActivityOriginalPrice = p.ActivityOriginalPrice,
                        ActivityRouteId = p.ActivityRouteId,
                        ActivityScenetype = p.ActivityScenetype,
                        ActivityStartTime = p.ActivityStartTime,
                        ActivityTenantName = p.ActivityTenantName,
                        ActivityTitle = p.ActivityTitle,
                        ActivityType = p.ActivityType,
                        ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                        CreateTime = p.CreateTime,
                        EtActivityRouteItemId = p.EtActivityRouteItemId,
                        IsTeamLeader = p.IsTeamLeader,
                        MiniPgmUserId = p.MiniPgmUserId,
                        NickName = p.NickName,
                        PayFinishTime = p.PayFinishTime,
                        PaySum = p.PaySum,
                        ScenetypeStyleClass = p.ScenetypeStyleClass,
                        StudentFieldValue1 = p.StudentFieldValue1,
                        StudentFieldValue2 = p.StudentFieldValue2,
                        StudentName = p.StudentName,
                        StudentPhone = p.StudentPhone,
                        TenantId = p.TenantId,
                        Status = p.Status
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<WxMiniActivityRouteItemGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> WxMiniActivityHomeGet(WxMiniActivityHomeGetRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (p == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            switch (p.ActivityType)
            {
                case EmActivityType.GroupPurchase:
                    return await WxMiniActivityHomeGetGroupPurchase(request, p);
                case EmActivityType.Haggling:
                    return await WxMiniActivityHomeGetHaggling(request, p);
            }
            return ResponseBase.CommonError("无法展示活动信息");
        }

        private async Task<ResponseBase> WxMiniActivityHomeGetGroupPurchase(WxMiniActivityHomeGetRequest request, EtActivityMain p)
        {
            var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(p.RuleContent);
            var totalLimitCount = ruleContent.Item.Sum(j => j.LimitCount);
            var maxCount = ruleContent.Item.Last().LimitCount;
            var groupPurchaseRule = new WxMiniActivityHomeGroupPurchaseRule()
            {
                Items = ruleContent.Item.Select(j => new WxMiniActivityHomeGroupPurchaseRuleItem()
                {
                    LimitCount = j.LimitCount,
                    Money = j.Money,
                    Length = j.LimitCount / Convert.ToDecimal(maxCount) * 100
                }).OrderBy(a => a.LimitCount).ToList(),
                TotalLimitCount = totalLimitCount
            };
            var activityStatusResult = EmActivityStatus.GetActivityStatus(p.ActivityStatus, p.EndTime.Value);
            var payInfo = ComBusiness5.GetActivityPayInfo(p, ruleContent);
            var output = new WxMiniActivityHomeGetOutput()
            {
                WxMiniTenantConfig = new WxMiniTenantConfig()
                {
                    MicroWebHomeUrl = AddressLib.GetMicroWebHomeUrl(_appConfigurtaionServices.AppSettings.SysAddressConfig.MicroWebHomeUrl, p.TenantId)
                },
                BascInfo = new WxMiniActivityHomeBascInfo()
                {
                    ActivityExplan = p.ActivityExplan,
                    ActivityMainId = p.Id,
                    ActivityStatus = activityStatusResult.Item1,
                    ActivityStatusDesc = activityStatusResult.Item2,
                    ActivityType = p.ActivityType,
                    ActivityTypeDesc = EmActivityType.GetActivityTypeDesc(p.ActivityType),
                    ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                    CourseDesc = p.CourseDesc,
                    CourseName = p.CourseName,
                    CoverImage = p.CoverImage,
                    CreateTime = p.CreateTime,
                    EndTime = p.EndTime.Value,
                    FailCount = p.FailCount,
                    FinishCount = p.FinishCount,
                    FinishFullCount = p.FinishFullCount,
                    GlobalOpenBullet = p.GlobalOpenBullet,
                    GlobalOpenStatistics = p.GlobalOpenStatistics,
                    GlobalPhone = p.GlobalPhone,
                    ImageCourse = EtmsHelper2.GetMediasUrl2(p.ImageCourse),
                    ImageMain = EtmsHelper2.GetMediasUrl2(p.ImageMain),
                    IsOpenCheckPhone = p.IsOpenCheckPhone,
                    IsOpenPay = p.IsOpenPay,
                    JoinCount = p.JoinCount,
                    MaxCount = p.MaxCount,
                    Name = p.Name,
                    OriginalPrice = p.OriginalPrice.EtmsToString3(),
                    PayType = p.PayType,
                    PayValue = p.PayValue,
                    PublishTime = p.PublishTime,
                    PVCount = p.PVCount,
                    RouteCount = p.RouteCount,
                    RuleEx1 = p.RuleEx1,
                    RuleEx2 = p.RuleEx2,
                    RuleEx3 = p.RuleEx3,
                    RuningCount = p.RuningCount,
                    Scenetype = p.Scenetype,
                    ScenetypeStyleClass = p.ScenetypeStyleClass,
                    ShareQRCode = p.ShareQRCode,
                    StartTime = p.StartTime,
                    StudentFieldName1 = p.StudentFieldName1,
                    StudentFieldName2 = p.StudentFieldName2,
                    StudentHisLimitType = p.StudentHisLimitType,
                    StyleBackColor = p.StyleBackColor,
                    StyleColumnColor = p.StyleColumnColor,
                    StyleColumnImg = p.StyleColumnImg,
                    StyleType = p.StyleType,
                    TenantId = p.TenantId,
                    TenantIntroduceImg = EtmsHelper2.GetMediasUrl2(p.TenantIntroduceImg),
                    TenantIntroduceTxt = p.TenantIntroduceTxt,
                    TenantLinkInfo = p.TenantLinkInfo,
                    TenantLinkQRcode = p.TenantLinkQRcode,
                    TenantName = p.TenantName,
                    Title = p.Title,
                    TranspondCount = p.TranspondCount,
                    UVCount = p.UVCount,
                    VisitCount = p.VisitCount,
                    GroupPurchaseRule = groupPurchaseRule,
                    ActivityRouteId = null,
                    ActivityRouteItemId = null,
                    IsMultiGroupPurchase = groupPurchaseRule.Items.Count > 1,
                    PayPriceDesc = payInfo.Item1,
                    PayMustValue = payInfo.Item2
                }
            };
            var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(request.ActivityMainId, request.MiniPgmUserId);
            if (myActivityRouteItem != null)
            {
                output.IsActivitying = true;
                output.ShareQRCode = myActivityRouteItem.ShareQRCode;
                output.BascInfo.ActivityRouteId = myActivityRouteItem.ActivityRouteId;
                output.BascInfo.ActivityRouteItemId = myActivityRouteItem.Id;
                var teamLeaderRoute = await _activityRouteDAL.GetActivityRoute(myActivityRouteItem.ActivityRouteId);
                var routeLimitResult = ComBusiness5.GetActivityRouteLimit(teamLeaderRoute.CountLimit, maxCount,
                    teamLeaderRoute.CountFinish);
                var myTeamLeaderRoute = new WxMiniActivityHomeMyRoute()
                {
                    ActivityRouteId = teamLeaderRoute.Id,
                    AvatarUrl = teamLeaderRoute.AvatarUrl,
                    CountFinish = teamLeaderRoute.CountFinish,
                    CountLimit = teamLeaderRoute.CountLimit,
                    MiniPgmUserId = teamLeaderRoute.MiniPgmUserId,
                    StudentNameDesc = EtmsHelper.GetNameSecrecy(teamLeaderRoute.StudentName),
                    CountShort = routeLimitResult.Item1,
                    CountShortStatus = routeLimitResult.Item2
                };
                var timeSurplus = EtmsHelper2.GetCountDown(teamLeaderRoute.ActivityEndTime);
                myTeamLeaderRoute.SurplusHour = timeSurplus.Item1;
                myTeamLeaderRoute.SurplusMinute = timeSurplus.Item2;
                myTeamLeaderRoute.SurplusSecond = timeSurplus.Item3;

                var teamLeaderRouteBucket = await _activityRouteDAL.GetActivityRouteBucket(teamLeaderRoute.Id);
                if (teamLeaderRouteBucket != null && teamLeaderRouteBucket.ActivityRouteItems != null && teamLeaderRouteBucket.ActivityRouteItems.Any())
                {
                    myTeamLeaderRoute.JoinRouteItems = new List<WxMiniActivityHomeJoinRouteItemSmall>();
                    foreach (var myActivityRouteItems in teamLeaderRouteBucket.ActivityRouteItems)
                    {
                        myTeamLeaderRoute.JoinRouteItems.Add(new WxMiniActivityHomeJoinRouteItemSmall()
                        {
                            ActivityRouteId = myActivityRouteItems.ActivityRouteId,
                            ActivityRouteItemId = myActivityRouteItems.Id,
                            AvatarUrl = myActivityRouteItems.AvatarUrl,
                            IsTeamLeader = myActivityRouteItems.IsTeamLeader,
                            MiniPgmUserId = myActivityRouteItems.MiniPgmUserId
                        });
                    }
                }
                output.TeamLeaderRoute = myTeamLeaderRoute;
            }
            var myActivityRouteTop10 = await _activityRouteDAL.ActivityRouteTop10(request.ActivityMainId);
            output.JoinRouteItems = new List<WxMiniActivityHomeJoinRoute>();
            if (myActivityRouteTop10.Any())
            {
                foreach (var item in myActivityRouteTop10)
                {
                    var myJoinRouteLimitResult = ComBusiness5.GetActivityRouteLimit(item.CountLimit, maxCount,
                        item.CountFinish);
                    var tempStudentNameDesc = EtmsHelper.GetNameSecrecy(item.StudentName);
                    var myJoinRoute = new WxMiniActivityHomeJoinRoute()
                    {
                        ActivityRouteId = item.Id,
                        AvatarUrl = item.AvatarUrl,
                        CurrentAvatarUrl = item.AvatarUrl,
                        CurrentStudentNameDesc = tempStudentNameDesc,
                        CountFinish = item.CountFinish,
                        CountLimit = item.CountLimit,
                        MiniPgmUserId = item.MiniPgmUserId,
                        StudentNameDesc = tempStudentNameDesc,
                        CountShort = myJoinRouteLimitResult.Item1,
                        CountShortStatus = myJoinRouteLimitResult.Item2,
                        JoinRouteItems = new List<WxMiniActivityHomeJoinRouteItem>()
                    };
                    var itemDetailBucket = await _activityRouteDAL.GetActivityRouteBucket(item.Id);
                    if (itemDetailBucket != null && itemDetailBucket.ActivityRouteItems != null &&
                        itemDetailBucket.ActivityRouteItems.Any())
                    {
                        foreach (var routeItem in itemDetailBucket.ActivityRouteItems)
                        {
                            myJoinRoute.JoinRouteItems.Add(new WxMiniActivityHomeJoinRouteItem()
                            {
                                ActivityRouteId = routeItem.ActivityRouteId,
                                ActivityRouteItemId = routeItem.Id,
                                AvatarUrl = routeItem.AvatarUrl,
                                IsTeamLeader = routeItem.IsTeamLeader,
                                MiniPgmUserId = routeItem.MiniPgmUserId,
                                StudentNameDesc = EtmsHelper.GetNameSecrecy(routeItem.StudentName)
                            });
                        }
                    }
                    output.JoinRouteItems.Add(myJoinRoute);
                }
            }
            return ResponseBase.Success(output);
        }

        private async Task<ResponseBase> WxMiniActivityHomeGetHaggling(WxMiniActivityHomeGetRequest request, EtActivityMain p)
        {
            var activityStatusResult = EmActivityStatus.GetActivityStatus(p.ActivityStatus, p.EndTime.Value);
            var output = new WxMiniActivityHomeGetOutput()
            {
                WxMiniTenantConfig = new WxMiniTenantConfig()
                {
                    MicroWebHomeUrl = AddressLib.GetMicroWebHomeUrl(_appConfigurtaionServices.AppSettings.SysAddressConfig.MicroWebHomeUrl, p.TenantId)
                },
                BascInfo = new WxMiniActivityHomeBascInfo()
                {
                    ActivityExplan = p.ActivityExplan,
                    ActivityMainId = p.Id,
                    ActivityStatus = activityStatusResult.Item1,
                    ActivityStatusDesc = activityStatusResult.Item2,
                    ActivityType = p.ActivityType,
                    ActivityTypeDesc = EmActivityType.GetActivityTypeDesc(p.ActivityType),
                    ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                    CourseDesc = p.CourseDesc,
                    CourseName = p.CourseName,
                    CoverImage = p.CoverImage,
                    CreateTime = p.CreateTime,
                    EndTime = p.EndTime.Value,
                    FailCount = p.FailCount,
                    FinishCount = p.FinishCount,
                    FinishFullCount = p.FinishFullCount,
                    GlobalOpenBullet = p.GlobalOpenBullet,
                    GlobalOpenStatistics = p.GlobalOpenStatistics,
                    GlobalPhone = p.GlobalPhone,
                    ImageCourse = EtmsHelper2.GetMediasUrl2(p.ImageCourse),
                    ImageMain = EtmsHelper2.GetMediasUrl2(p.ImageMain),
                    IsOpenCheckPhone = p.IsOpenCheckPhone,
                    IsOpenPay = p.IsOpenPay,
                    JoinCount = p.JoinCount,
                    MaxCount = p.MaxCount,
                    Name = p.Name,
                    OriginalPrice = p.OriginalPrice.EtmsToString3(),
                    PayType = p.PayType,
                    PayValue = p.PayValue,
                    PublishTime = p.PublishTime,
                    PVCount = p.PVCount,
                    RouteCount = p.RouteCount,
                    RuleEx1 = p.RuleEx1,
                    RuleEx2 = p.RuleEx2,
                    RuleEx3 = p.RuleEx3,
                    RuningCount = p.RuningCount,
                    Scenetype = p.Scenetype,
                    ScenetypeStyleClass = p.ScenetypeStyleClass,
                    ShareQRCode = p.ShareQRCode,
                    StartTime = p.StartTime,
                    StudentFieldName1 = p.StudentFieldName1,
                    StudentFieldName2 = p.StudentFieldName2,
                    StudentHisLimitType = p.StudentHisLimitType,
                    StyleBackColor = p.StyleBackColor,
                    StyleColumnColor = p.StyleColumnColor,
                    StyleColumnImg = p.StyleColumnImg,
                    StyleType = p.StyleType,
                    TenantId = p.TenantId,
                    TenantIntroduceImg = EtmsHelper2.GetMediasUrl2(p.TenantIntroduceImg),
                    TenantIntroduceTxt = p.TenantIntroduceTxt,
                    TenantLinkInfo = p.TenantLinkInfo,
                    TenantLinkQRcode = p.TenantLinkQRcode,
                    TenantName = p.TenantName,
                    Title = p.Title,
                    TranspondCount = p.TranspondCount,
                    UVCount = p.UVCount,
                    VisitCount = p.VisitCount,
                    GroupPurchaseRule = null,
                    ActivityRouteId = null,
                    ActivityRouteItemId = null
                }
            };
            var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(request.ActivityMainId, request.MiniPgmUserId);
            if (myActivityRouteItem != null)
            {
                output.IsActivitying = true;
                output.HaggleLogStatus = HaggleLogStatusOutput.My;
                output.ShareQRCode = myActivityRouteItem.ShareQRCode;
                output.BascInfo.ActivityRouteId = myActivityRouteItem.ActivityRouteId;
                output.BascInfo.ActivityRouteItemId = myActivityRouteItem.Id;
                var teamLeaderRoute = await _activityRouteDAL.GetActivityRoute(myActivityRouteItem.ActivityRouteId);
                var myTeamLeaderRoute = new WxMiniActivityHomeMyRoute()
                {
                    ActivityRouteId = teamLeaderRoute.Id,
                    AvatarUrl = teamLeaderRoute.AvatarUrl,
                    CountFinish = teamLeaderRoute.CountFinish,
                    CountLimit = teamLeaderRoute.CountLimit,
                    MiniPgmUserId = teamLeaderRoute.MiniPgmUserId,
                    StudentNameDesc = EtmsHelper.GetNameSecrecy(teamLeaderRoute.StudentName)
                };
                var timeSurplus = EtmsHelper2.GetCountDown(teamLeaderRoute.ActivityEndTime);
                myTeamLeaderRoute.SurplusHour = timeSurplus.Item1;
                myTeamLeaderRoute.SurplusMinute = timeSurplus.Item2;
                myTeamLeaderRoute.SurplusSecond = timeSurplus.Item3;
                output.TeamLeaderRoute = myTeamLeaderRoute;

                var myHaggleLog = await _activityRouteDAL.GetActivityHaggleLog(myActivityRouteItem.ActivityId, myActivityRouteItem.ActivityRouteId, request.MiniPgmUserId);
                if (myHaggleLog == null)
                {
                    output.IsCanHaggleLog = true;
                }
                else
                {
                    var myRepeatHaggleHour = p.RuleEx3.ToInt();
                    if (myRepeatHaggleHour != 0)
                    {
                        var nextTime = myHaggleLog.CreateTime.AddHours(myRepeatHaggleHour);
                        if (DateTime.Now >= nextTime)
                        {
                            output.IsCanHaggleLog = true;
                        }
                    }
                }

                output.HaggleLogs = new List<WxMiniActivityHomeHaggleLog>();
                var myActivityRouteBucket = await _activityRouteDAL.GetActivityRouteBucket(myActivityRouteItem.ActivityRouteId);
                if (myActivityRouteBucket != null && myActivityRouteBucket.ActivityHaggleLogs != null && myActivityRouteBucket.ActivityHaggleLogs.Any())
                {
                    foreach (var haggleLogsItem in myActivityRouteBucket.ActivityHaggleLogs)
                    {
                        output.HaggleLogs.Add(new WxMiniActivityHomeHaggleLog()
                        {
                            AvatarUrl = haggleLogsItem.AvatarUrl,
                            MiniPgmUserId = haggleLogsItem.MiniPgmUserId,
                            NickName = haggleLogsItem.NickName
                        });
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> WxMiniActivityHomeGet2(WxMiniActivityHomeGet2Request request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var myActivityRouteItem = await _activityRouteDAL.GetActivityRouteItem(request.ActivityRouteItemId);
            if (myActivityRouteItem == null)
            {
                return ResponseBase.CommonError("活动参与记录不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(myActivityRouteItem.ActivityId);
            if (p == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            switch (p.ActivityType)
            {
                case EmActivityType.GroupPurchase:
                    return await WxMiniActivityHomeGet2GroupPurchase(request, p, myActivityRouteItem);
                case EmActivityType.Haggling:
                    return await WxMiniActivityHomeGet2Haggling(request, p, myActivityRouteItem);
            }
            return ResponseBase.CommonError("无法展示活动信息");
        }

        public async Task<ResponseBase> WxMiniActivityRouteItemMoreGetPaging(WxMiniActivityRouteItemMoreGetPagingRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myActivityMain = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (myActivityMain == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(myActivityMain.RuleContent);
            var totalLimitCount = ruleContent.Item.Sum(j => j.LimitCount);
            var maxCount = ruleContent.Item.Last().LimitCount;

            var output = new List<WxMiniActivityRouteItemMoreGetPagingOutput>();
            var pagingData = await _activityRouteDAL.GetPagingRoute(request);
            if (pagingData.Item1.Any())
            {
                foreach (var item in pagingData.Item1)
                {
                    var myJoinRouteLimitResult = ComBusiness5.GetActivityRouteLimit(item.CountLimit, maxCount,
                                           item.CountFinish);
                    var tempStudentNameDesc = EtmsHelper.GetNameSecrecy(item.StudentName);
                    var myJoinRoute = new WxMiniActivityRouteItemMoreGetPagingOutput()
                    {
                        ActivityRouteId = item.Id,
                        AvatarUrl = item.AvatarUrl,
                        CurrentAvatarUrl = item.AvatarUrl,
                        CurrentStudentNameDesc = tempStudentNameDesc,
                        CountFinish = item.CountFinish,
                        CountLimit = item.CountLimit,
                        MiniPgmUserId = item.MiniPgmUserId,
                        StudentNameDesc = tempStudentNameDesc,
                        CountShort = myJoinRouteLimitResult.Item1,
                        CountShortStatus = myJoinRouteLimitResult.Item2,
                        JoinRouteItems = new List<WxMiniActivityHomeJoinRouteItem>()
                    };
                    var itemDetailBucket = await _activityRouteDAL.GetActivityRouteBucket(item.Id);
                    if (itemDetailBucket != null && itemDetailBucket.ActivityRouteItems != null &&
                        itemDetailBucket.ActivityRouteItems.Any())
                    {
                        foreach (var routeItem in itemDetailBucket.ActivityRouteItems)
                        {
                            myJoinRoute.JoinRouteItems.Add(new WxMiniActivityHomeJoinRouteItem()
                            {
                                ActivityRouteId = routeItem.ActivityRouteId,
                                ActivityRouteItemId = routeItem.Id,
                                AvatarUrl = routeItem.AvatarUrl,
                                IsTeamLeader = routeItem.IsTeamLeader,
                                MiniPgmUserId = routeItem.MiniPgmUserId,
                                StudentNameDesc = EtmsHelper.GetNameSecrecy(routeItem.StudentName)
                            });
                        }
                    }
                    output.Add(myJoinRoute);
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<WxMiniActivityRouteItemMoreGetPagingOutput>(pagingData.Item2, output));
        }

        private async Task<ResponseBase> WxMiniActivityHomeGet2GroupPurchase(WxMiniActivityHomeGet2Request request, EtActivityMain p, EtActivityRouteItem myActivityRouteItemLeader)
        {
            var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(p.RuleContent);
            var totalLimitCount = ruleContent.Item.Sum(j => j.LimitCount);
            var maxCount = ruleContent.Item.Last().LimitCount;
            var groupPurchaseRule = new WxMiniActivityHomeGroupPurchaseRule()
            {
                Items = ruleContent.Item.Select(j => new WxMiniActivityHomeGroupPurchaseRuleItem()
                {
                    LimitCount = j.LimitCount,
                    Money = j.Money,
                    Length = j.LimitCount / Convert.ToDecimal(maxCount) * 100
                }).OrderBy(a => a.LimitCount).ToList(),
                TotalLimitCount = totalLimitCount
            };
            var activityStatusResult = EmActivityStatus.GetActivityStatus(p.ActivityStatus, p.EndTime.Value);
            var payInfo = ComBusiness5.GetActivityPayInfo(p, ruleContent);
            var output = new WxMiniActivityHomeGetOutput()
            {
                WxMiniTenantConfig = new WxMiniTenantConfig()
                {
                    MicroWebHomeUrl = AddressLib.GetMicroWebHomeUrl(_appConfigurtaionServices.AppSettings.SysAddressConfig.MicroWebHomeUrl, p.TenantId)
                },
                BascInfo = new WxMiniActivityHomeBascInfo()
                {
                    ActivityExplan = p.ActivityExplan,
                    ActivityMainId = p.Id,
                    ActivityStatus = activityStatusResult.Item1,
                    ActivityStatusDesc = activityStatusResult.Item2,
                    ActivityType = p.ActivityType,
                    ActivityTypeDesc = EmActivityType.GetActivityTypeDesc(p.ActivityType),
                    ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                    CourseDesc = p.CourseDesc,
                    CourseName = p.CourseName,
                    CoverImage = p.CoverImage,
                    CreateTime = p.CreateTime,
                    EndTime = p.EndTime.Value,
                    FailCount = p.FailCount,
                    FinishFullCount = p.FinishFullCount,
                    FinishCount = p.FinishCount,
                    GlobalOpenBullet = p.GlobalOpenBullet,
                    GlobalOpenStatistics = p.GlobalOpenStatistics,
                    GlobalPhone = p.GlobalPhone,
                    ImageCourse = EtmsHelper2.GetMediasUrl2(p.ImageCourse),
                    ImageMain = EtmsHelper2.GetMediasUrl2(p.ImageMain),
                    IsOpenCheckPhone = p.IsOpenCheckPhone,
                    IsOpenPay = p.IsOpenPay,
                    JoinCount = p.JoinCount,
                    MaxCount = p.MaxCount,
                    Name = p.Name,
                    OriginalPrice = p.OriginalPrice.EtmsToString3(),
                    PayType = p.PayType,
                    PayValue = p.PayValue,
                    PublishTime = p.PublishTime,
                    PVCount = p.PVCount,
                    RouteCount = p.RouteCount,
                    RuleEx1 = p.RuleEx1,
                    RuleEx2 = p.RuleEx2,
                    RuleEx3 = p.RuleEx3,
                    RuningCount = p.RuningCount,
                    Scenetype = p.Scenetype,
                    ScenetypeStyleClass = p.ScenetypeStyleClass,
                    ShareQRCode = p.ShareQRCode,
                    StartTime = p.StartTime,
                    StudentFieldName1 = p.StudentFieldName1,
                    StudentFieldName2 = p.StudentFieldName2,
                    StudentHisLimitType = p.StudentHisLimitType,
                    StyleBackColor = p.StyleBackColor,
                    StyleColumnColor = p.StyleColumnColor,
                    StyleColumnImg = p.StyleColumnImg,
                    StyleType = p.StyleType,
                    TenantId = p.TenantId,
                    TenantIntroduceImg = EtmsHelper2.GetMediasUrl2(p.TenantIntroduceImg),
                    TenantIntroduceTxt = p.TenantIntroduceTxt,
                    TenantLinkInfo = p.TenantLinkInfo,
                    TenantLinkQRcode = p.TenantLinkQRcode,
                    TenantName = p.TenantName,
                    Title = p.Title,
                    TranspondCount = p.TranspondCount,
                    UVCount = p.UVCount,
                    VisitCount = p.VisitCount,
                    GroupPurchaseRule = groupPurchaseRule,
                    ActivityRouteId = null,
                    ActivityRouteItemId = null,
                    IsMultiGroupPurchase = groupPurchaseRule.Items.Count > 1,
                    PayPriceDesc = payInfo.Item1,
                    PayMustValue = payInfo.Item2
                }
            };
            output.IsActivitying = false;
            output.ShareQRCode = myActivityRouteItemLeader.ShareQRCode;
            output.BascInfo.ActivityRouteId = myActivityRouteItemLeader.ActivityRouteId;
            output.BascInfo.ActivityRouteItemId = myActivityRouteItemLeader.Id;
            var teamLeaderRoute = await _activityRouteDAL.GetActivityRoute(myActivityRouteItemLeader.ActivityRouteId);
            var myLeaderLimitResult = ComBusiness5.GetActivityRouteLimit(teamLeaderRoute.CountLimit, maxCount,
                        teamLeaderRoute.CountFinish);
            var myTeamLeaderRoute = new WxMiniActivityHomeMyRoute()
            {
                ActivityRouteId = teamLeaderRoute.Id,
                AvatarUrl = teamLeaderRoute.AvatarUrl,
                CountFinish = teamLeaderRoute.CountFinish,
                CountLimit = teamLeaderRoute.CountLimit,
                MiniPgmUserId = teamLeaderRoute.MiniPgmUserId,
                StudentNameDesc = EtmsHelper.GetNameSecrecy(teamLeaderRoute.StudentName),
                CountShort = myLeaderLimitResult.Item1,
                CountShortStatus = myLeaderLimitResult.Item2
            };
            var timeSurplus = EtmsHelper2.GetCountDown(teamLeaderRoute.ActivityEndTime);
            myTeamLeaderRoute.SurplusHour = timeSurplus.Item1;
            myTeamLeaderRoute.SurplusMinute = timeSurplus.Item2;
            myTeamLeaderRoute.SurplusSecond = timeSurplus.Item3;
            var teamLeaderRouteBucket = await _activityRouteDAL.GetActivityRouteBucket(teamLeaderRoute.Id);
            if (teamLeaderRouteBucket != null && teamLeaderRouteBucket.ActivityRouteItems != null && teamLeaderRouteBucket.ActivityRouteItems.Any())
            {
                myTeamLeaderRoute.JoinRouteItems = new List<WxMiniActivityHomeJoinRouteItemSmall>();
                foreach (var myActivityRouteItems in teamLeaderRouteBucket.ActivityRouteItems)
                {
                    myTeamLeaderRoute.JoinRouteItems.Add(new WxMiniActivityHomeJoinRouteItemSmall()
                    {
                        ActivityRouteId = myActivityRouteItems.ActivityRouteId,
                        ActivityRouteItemId = myActivityRouteItems.Id,
                        AvatarUrl = myActivityRouteItems.AvatarUrl,
                        IsTeamLeader = myActivityRouteItems.IsTeamLeader,
                        MiniPgmUserId = myActivityRouteItems.MiniPgmUserId
                    });
                }
            }
            output.TeamLeaderRoute = myTeamLeaderRoute;
            var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(p.Id, request.MiniPgmUserId);
            if (myActivityRouteItem != null)
            {
                output.IsActivitying = true;
                output.ShareQRCode = myActivityRouteItem.ShareQRCode;
                output.BascInfo.ActivityRouteId = myActivityRouteItem.ActivityRouteId;
                output.BascInfo.ActivityRouteItemId = myActivityRouteItem.Id;
            }

            var myActivityRouteTop10 = await _activityRouteDAL.ActivityRouteTop10(p.Id);
            output.JoinRouteItems = new List<WxMiniActivityHomeJoinRoute>();
            if (myActivityRouteTop10.Any())
            {
                foreach (var item in myActivityRouteTop10)
                {
                    var myJoinRouteLimitResult = ComBusiness5.GetActivityRouteLimit(item.CountLimit, maxCount,
                        item.CountFinish);
                    var tempStudentNameDesc = EtmsHelper.GetNameSecrecy(item.StudentName);
                    var myJoinRoute = new WxMiniActivityHomeJoinRoute()
                    {
                        ActivityRouteId = item.Id,
                        AvatarUrl = item.AvatarUrl,
                        CurrentAvatarUrl = item.AvatarUrl,
                        CurrentStudentNameDesc = tempStudentNameDesc,
                        CountFinish = item.CountFinish,
                        CountLimit = item.CountLimit,
                        MiniPgmUserId = item.MiniPgmUserId,
                        StudentNameDesc = tempStudentNameDesc,
                        CountShort = myJoinRouteLimitResult.Item1,
                        CountShortStatus = myJoinRouteLimitResult.Item2,
                        JoinRouteItems = new List<WxMiniActivityHomeJoinRouteItem>()
                    };
                    var itemDetailBucket = await _activityRouteDAL.GetActivityRouteBucket(item.Id);
                    if (itemDetailBucket != null && itemDetailBucket.ActivityRouteItems != null &&
                        itemDetailBucket.ActivityRouteItems.Any())
                    {
                        foreach (var routeItem in itemDetailBucket.ActivityRouteItems)
                        {
                            myJoinRoute.JoinRouteItems.Add(new WxMiniActivityHomeJoinRouteItem()
                            {
                                ActivityRouteId = routeItem.ActivityRouteId,
                                ActivityRouteItemId = routeItem.Id,
                                AvatarUrl = routeItem.AvatarUrl,
                                IsTeamLeader = routeItem.IsTeamLeader,
                                MiniPgmUserId = routeItem.MiniPgmUserId,
                                StudentNameDesc = EtmsHelper.GetNameSecrecy(routeItem.StudentName)
                            });
                        }
                    }
                    output.JoinRouteItems.Add(myJoinRoute);
                }
            }
            return ResponseBase.Success(output);
        }

        private async Task<ResponseBase> WxMiniActivityHomeGet2Haggling(WxMiniActivityHomeGet2Request request, EtActivityMain p, EtActivityRouteItem myActivityRouteItemLeader)
        {
            var activityStatusResult = EmActivityStatus.GetActivityStatus(p.ActivityStatus, p.EndTime.Value);
            var output = new WxMiniActivityHomeGetOutput()
            {
                WxMiniTenantConfig = new WxMiniTenantConfig()
                {
                    MicroWebHomeUrl = AddressLib.GetMicroWebHomeUrl(_appConfigurtaionServices.AppSettings.SysAddressConfig.MicroWebHomeUrl, p.TenantId)
                },
                BascInfo = new WxMiniActivityHomeBascInfo()
                {
                    ActivityExplan = p.ActivityExplan,
                    ActivityMainId = p.Id,
                    ActivityStatus = activityStatusResult.Item1,
                    ActivityStatusDesc = activityStatusResult.Item2,
                    ActivityType = p.ActivityType,
                    ActivityTypeDesc = EmActivityType.GetActivityTypeDesc(p.ActivityType),
                    ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                    CourseDesc = p.CourseDesc,
                    CourseName = p.CourseName,
                    CoverImage = p.CoverImage,
                    CreateTime = p.CreateTime,
                    EndTime = p.EndTime.Value,
                    FailCount = p.FailCount,
                    FinishCount = p.FinishCount,
                    FinishFullCount = p.FinishFullCount,
                    GlobalOpenBullet = p.GlobalOpenBullet,
                    GlobalOpenStatistics = p.GlobalOpenStatistics,
                    GlobalPhone = p.GlobalPhone,
                    ImageCourse = EtmsHelper2.GetMediasUrl2(p.ImageCourse),
                    ImageMain = EtmsHelper2.GetMediasUrl2(p.ImageMain),
                    IsOpenCheckPhone = p.IsOpenCheckPhone,
                    IsOpenPay = p.IsOpenPay,
                    JoinCount = p.JoinCount,
                    MaxCount = p.MaxCount,
                    Name = p.Name,
                    OriginalPrice = p.OriginalPrice.EtmsToString3(),
                    PayType = p.PayType,
                    PayValue = p.PayValue,
                    PublishTime = p.PublishTime,
                    PVCount = p.PVCount,
                    RouteCount = p.RouteCount,
                    RuleEx1 = p.RuleEx1,
                    RuleEx2 = p.RuleEx2,
                    RuleEx3 = p.RuleEx3,
                    RuningCount = p.RuningCount,
                    Scenetype = p.Scenetype,
                    ScenetypeStyleClass = p.ScenetypeStyleClass,
                    ShareQRCode = p.ShareQRCode,
                    StartTime = p.StartTime,
                    StudentFieldName1 = p.StudentFieldName1,
                    StudentFieldName2 = p.StudentFieldName2,
                    StudentHisLimitType = p.StudentHisLimitType,
                    StyleBackColor = p.StyleBackColor,
                    StyleColumnColor = p.StyleColumnColor,
                    StyleColumnImg = p.StyleColumnImg,
                    StyleType = p.StyleType,
                    TenantId = p.TenantId,
                    TenantIntroduceImg = EtmsHelper2.GetMediasUrl2(p.TenantIntroduceImg),
                    TenantIntroduceTxt = p.TenantIntroduceTxt,
                    TenantLinkInfo = p.TenantLinkInfo,
                    TenantLinkQRcode = p.TenantLinkQRcode,
                    TenantName = p.TenantName,
                    Title = p.Title,
                    TranspondCount = p.TranspondCount,
                    UVCount = p.UVCount,
                    VisitCount = p.VisitCount,
                    GroupPurchaseRule = null,
                    ActivityRouteId = null,
                    ActivityRouteItemId = null
                }
            };

            output.IsActivitying = true;
            output.ShareQRCode = myActivityRouteItemLeader.ShareQRCode;
            output.BascInfo.ActivityRouteId = myActivityRouteItemLeader.ActivityRouteId;
            output.BascInfo.ActivityRouteItemId = myActivityRouteItemLeader.Id;
            var teamLeaderRoute = await _activityRouteDAL.GetActivityRoute(myActivityRouteItemLeader.ActivityRouteId);
            var myTeamLeaderRoute = new WxMiniActivityHomeMyRoute()
            {
                ActivityRouteId = teamLeaderRoute.Id,
                AvatarUrl = teamLeaderRoute.AvatarUrl,
                CountFinish = teamLeaderRoute.CountFinish,
                CountLimit = teamLeaderRoute.CountLimit,
                MiniPgmUserId = teamLeaderRoute.MiniPgmUserId,
                StudentNameDesc = EtmsHelper.GetNameSecrecy(teamLeaderRoute.StudentName)
            };
            var timeSurplus = EtmsHelper2.GetCountDown(teamLeaderRoute.ActivityEndTime);
            myTeamLeaderRoute.SurplusHour = timeSurplus.Item1;
            myTeamLeaderRoute.SurplusMinute = timeSurplus.Item2;
            myTeamLeaderRoute.SurplusSecond = timeSurplus.Item3;
            output.TeamLeaderRoute = myTeamLeaderRoute;

            if (myActivityRouteItemLeader.MiniPgmUserId == request.MiniPgmUserId)
            {
                output.HaggleLogStatus = HaggleLogStatusOutput.My;
            }
            else
            {
                var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(p.Id, request.MiniPgmUserId);
                if (myActivityRouteItem != null)
                {
                    output.HaggleLogStatus = HaggleLogStatusOutput.OtherPeopleAndMyHave;
                }
                else
                {
                    output.HaggleLogStatus = HaggleLogStatusOutput.OtherPeopleAndMyNone;
                }
            }

            var myHaggleLog = await _activityRouteDAL.GetActivityHaggleLog(myActivityRouteItemLeader.ActivityId, myActivityRouteItemLeader.ActivityRouteId, request.MiniPgmUserId);
            if (myHaggleLog == null)
            {
                output.IsCanHaggleLog = true;
            }
            else
            {
                if (output.HaggleLogStatus == HaggleLogStatusOutput.My)
                {
                    var myRepeatHaggleHour = p.RuleEx3.ToInt();
                    if (myRepeatHaggleHour != 0)
                    {
                        var nextTime = myHaggleLog.CreateTime.AddHours(myRepeatHaggleHour);
                        if (DateTime.Now >= nextTime)
                        {
                            output.IsCanHaggleLog = true;
                        }
                    }
                }
            }

            output.HaggleLogs = new List<WxMiniActivityHomeHaggleLog>();
            var myActivityRouteBucket = await _activityRouteDAL.GetActivityRouteBucket(myActivityRouteItemLeader.ActivityRouteId);
            if (myActivityRouteBucket != null && myActivityRouteBucket.ActivityHaggleLogs != null && myActivityRouteBucket.ActivityHaggleLogs.Any())
            {
                foreach (var haggleLogsItem in myActivityRouteBucket.ActivityHaggleLogs)
                {
                    output.HaggleLogs.Add(new WxMiniActivityHomeHaggleLog()
                    {
                        AvatarUrl = haggleLogsItem.AvatarUrl,
                        MiniPgmUserId = haggleLogsItem.MiniPgmUserId,
                        NickName = haggleLogsItem.NickName
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> WxMiniActivityGroupPurchaseDiscount(WxMiniActivityGroupPurchaseDiscountRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myActivityRouteBucket = await _activityRouteDAL.GetActivityRouteBucket(request.ActivityRouteId);
            if (myActivityRouteBucket == null || myActivityRouteBucket.ActivityRoute == null)
            {
                return ResponseBase.CommonError("活动参与记录不存在");
            }
            var myActivityRoute = myActivityRouteBucket.ActivityRoute;
            var p = await _activityMainDAL.GetActivityMain(myActivityRoute.ActivityId);
            if (p == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(p.RuleContent);
            var totalLimitCount = ruleContent.Item.Sum(j => j.LimitCount);
            var maxCount = ruleContent.Item.Last().LimitCount;
            var groupPurchaseRule = new WxMiniActivityHomeGroupPurchaseRule()
            {
                Items = ruleContent.Item.Select(j => new WxMiniActivityHomeGroupPurchaseRuleItem()
                {
                    LimitCount = j.LimitCount,
                    Money = j.Money,
                    Length = j.LimitCount / Convert.ToDecimal(maxCount) * 100
                }).OrderBy(a => a.LimitCount).ToList(),
                TotalLimitCount = totalLimitCount
            };
            var myJoinRouteLimitResult = ComBusiness5.GetActivityRouteLimit(myActivityRoute.CountLimit, maxCount,
                      myActivityRoute.CountFinish);
            var tempStudentNameDesc = EtmsHelper.GetNameSecrecy(myActivityRoute.StudentName);
            var output = new WxMiniActivityGroupPurchaseDiscountOutput()
            {
                IsMultiGroupPurchase = groupPurchaseRule.Items.Count > 1,
                GroupPurchaseRule = groupPurchaseRule,
                JoinRoute = new WxMiniActivityHomeJoinRoute()
                {
                    ActivityRouteId = myActivityRoute.Id,
                    AvatarUrl = myActivityRoute.AvatarUrl,
                    CountFinish = myActivityRoute.CountFinish,
                    CountLimit = myActivityRoute.CountLimit,
                    MiniPgmUserId = myActivityRoute.MiniPgmUserId,
                    CurrentAvatarUrl = myActivityRoute.AvatarUrl,
                    CurrentStudentNameDesc = tempStudentNameDesc,
                    StudentNameDesc = tempStudentNameDesc,
                    CountShort = myJoinRouteLimitResult.Item1,
                    CountShortStatus = myJoinRouteLimitResult.Item2,
                    JoinRouteItems = new List<WxMiniActivityHomeJoinRouteItem>(),
                },
            };
            if (myActivityRouteBucket.ActivityRouteItems != null && myActivityRouteBucket.ActivityRouteItems.Any())
            {
                foreach (var routeItem in myActivityRouteBucket.ActivityRouteItems)
                {
                    output.JoinRoute.JoinRouteItems.Add(new WxMiniActivityHomeJoinRouteItem()
                    {
                        ActivityRouteId = routeItem.ActivityRouteId,
                        ActivityRouteItemId = routeItem.Id,
                        AvatarUrl = routeItem.AvatarUrl,
                        IsTeamLeader = routeItem.IsTeamLeader,
                        MiniPgmUserId = routeItem.MiniPgmUserId,
                        StudentNameDesc = EtmsHelper.GetNameSecrecy(routeItem.StudentName)
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> WxMiniActivityGetSimple(WxMiniActivityGetSimpleRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (p == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            var activityStatusResult = EmActivityStatus.GetActivityStatus(p.ActivityStatus, p.EndTime.Value);
            var config = await _tenantConfig2DAL.GetTenantConfig();
            var output = new WxMiniActivityGetSimpleOutput()
            {
                ActivityMainId = p.Id,
                ActivityStatus = activityStatusResult.Item1,
                ActivityStatusDesc = activityStatusResult.Item2,
                ActivityType = p.ActivityType,
                ActivityTypeDesc = EmActivityType.GetActivityTypeDesc(p.ActivityType),
                ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                CourseName = p.CourseName,
                CoverImage = p.CoverImage,
                CreateTime = p.CreateTime,
                EndTime = p.EndTime.Value,
                GlobalOpenBullet = p.GlobalOpenBullet,
                GlobalOpenStatistics = p.GlobalOpenStatistics,
                GlobalPhone = p.GlobalPhone,
                IsOpenCheckPhone = p.IsOpenCheckPhone,
                IsOpenPay = p.IsOpenPay,
                MaxCount = p.MaxCount,
                Name = p.Name,
                OriginalPrice = p.OriginalPrice,
                PayType = p.PayType,
                PayValue = p.PayValue,
                PublishTime = p.PublishTime,
                RuleEx1 = p.RuleEx1,
                RuleEx2 = p.RuleEx2,
                RuleEx3 = p.RuleEx3,
                Scenetype = p.Scenetype,
                ScenetypeStyleClass = p.ScenetypeStyleClass,
                ShareQRCode = p.ShareQRCode,
                StartTime = p.StartTime,
                StudentFieldName1 = p.StudentFieldName1,
                StudentFieldName2 = p.StudentFieldName2,
                StudentHisLimitType = p.StudentHisLimitType,
                StyleBackColor = p.StyleBackColor,
                StyleColumnColor = p.StyleColumnColor,
                StyleColumnImg = p.StyleColumnImg,
                StyleType = p.StyleType,
                TenantId = p.TenantId,
                TenantName = p.TenantName,
                Title = p.Title,
                IsMultiGroupPurchase = false,
                GroupPurchaseRule = null,
                ActivityConfig = config.ActivityConfig
            };
            if (p.ActivityType == EmActivityType.GroupPurchase)
            {
                var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(p.RuleContent);
                var totalLimitCount = ruleContent.Item.Sum(j => j.LimitCount);
                var maxCount = ruleContent.Item.Last().LimitCount;
                output.GroupPurchaseRule = new WxMiniActivityHomeGroupPurchaseRule()
                {
                    Items = ruleContent.Item.Select(j => new WxMiniActivityHomeGroupPurchaseRuleItem()
                    {
                        LimitCount = j.LimitCount,
                        Money = j.Money,
                        Length = j.LimitCount / Convert.ToDecimal(maxCount) * 100
                    }).OrderBy(a => a.LimitCount).ToList(),
                    TotalLimitCount = totalLimitCount
                };
                output.IsMultiGroupPurchase = output.GroupPurchaseRule.Items.Count > 1;
                var payInfo = ComBusiness5.GetActivityPayInfo(p, ruleContent);
                output.PayPriceDesc = payInfo.Item1;
                output.PayMustValue = payInfo.Item2;
                if (request.ActivityRouteId != null && request.ActivityRouteId > 0)
                {
                    var myActivityRouteBucket = await _activityRouteDAL.GetActivityRouteBucket(request.ActivityRouteId.Value);
                    if (myActivityRouteBucket != null)
                    {
                        var item = myActivityRouteBucket.ActivityRoute;
                        var myJoinRouteLimitResult = ComBusiness5.GetActivityRouteLimit(item.CountLimit, maxCount,
                            item.CountFinish);
                        var tempStudentNameDesc = EtmsHelper.GetNameSecrecy(item.StudentName);
                        var myJoinRoute = new WxMiniActivityHomeMyRoute()
                        {
                            ActivityRouteId = item.Id,
                            AvatarUrl = item.AvatarUrl,
                            CountFinish = item.CountFinish,
                            CountLimit = item.CountLimit,
                            MiniPgmUserId = item.MiniPgmUserId,
                            StudentNameDesc = tempStudentNameDesc,
                            CountShort = myJoinRouteLimitResult.Item1,
                            CountShortStatus = myJoinRouteLimitResult.Item2,
                            JoinRouteItems = new List<WxMiniActivityHomeJoinRouteItemSmall>()
                        };
                        if (myActivityRouteBucket.ActivityRouteItems != null && myActivityRouteBucket.ActivityRouteItems.Any())
                        {
                            foreach (var routeItem in myActivityRouteBucket.ActivityRouteItems)
                            {
                                myJoinRoute.JoinRouteItems.Add(new WxMiniActivityHomeJoinRouteItemSmall()
                                {
                                    ActivityRouteId = routeItem.ActivityRouteId,
                                    ActivityRouteItemId = routeItem.Id,
                                    AvatarUrl = routeItem.AvatarUrl,
                                    IsTeamLeader = routeItem.IsTeamLeader,
                                    MiniPgmUserId = routeItem.MiniPgmUserId
                                });
                            }
                        }
                        output.TeamLeaderRoute = myJoinRoute;
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> WxMiniActivityDynamicBulletGetPaging(WxMiniActivityDynamicBulletGetPagingRequest request)
        {
            this.InitTenantId(request.TenantId);
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            if (p == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            switch (p.ActivityType)
            {
                case EmActivityType.GroupPurchase:
                    return await WxMiniActivityDynamicBulletGetPagingGroupPurchase(request, p);
                case EmActivityType.Haggling:
                    return await WxMiniActivityDynamicBulletGetPagingHaggling(request, p);
            }
            return ResponseBase.CommonError("无法展示活动信息");
        }

        private async Task<ResponseBase> WxMiniActivityDynamicBulletGetPagingGroupPurchase(WxMiniActivityDynamicBulletGetPagingRequest request,
            EtActivityMain activityMain)
        {
            var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(activityMain.RuleContent);
            var maxCount = ruleContent.Item.Last().LimitCount;
            var pagingData = await _activityRouteDAL.GetPagingRouteItem(request);
            var output = new List<WxMiniActivityDynamicBulletGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    if (p.IsTeamLeader)
                    {
                        var myRouteItem = await _activityRouteDAL.GetActivityRoute(p.ActivityRouteId);
                        if (myRouteItem == null)
                        {
                            continue;
                        }
                        var myJoinRouteLimitResult = ComBusiness5.GetActivityRouteLimit(myRouteItem.CountLimit, maxCount,
                            myRouteItem.CountFinish);
                        output.Add(new WxMiniActivityDynamicBulletGetPagingOutput()
                        {
                            AvatarUrl = p.AvatarUrl,
                            StudentNameDesc = EtmsHelper.GetNameSecrecy(p.StudentName),
                            TagDesc = EmActivityRouteCountStatus.ActivityRouteCountStatusTag(myJoinRouteLimitResult.Item2)
                        });
                    }
                    else
                    {
                        output.Add(new WxMiniActivityDynamicBulletGetPagingOutput()
                        {
                            AvatarUrl = p.AvatarUrl,
                            StudentNameDesc = EtmsHelper.GetNameSecrecy(p.StudentName),
                            TagDesc = "参团成功"
                        });
                    }
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<WxMiniActivityDynamicBulletGetPagingOutput>(pagingData.Item2, output));
        }

        private async Task<ResponseBase> WxMiniActivityDynamicBulletGetPagingHaggling(WxMiniActivityDynamicBulletGetPagingRequest request,
            EtActivityMain activityMain)
        {
            var pagingData = await _activityRouteDAL.GetPagingRoute(request);
            var output = new List<WxMiniActivityDynamicBulletGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new WxMiniActivityDynamicBulletGetPagingOutput()
                    {
                        AvatarUrl = p.AvatarUrl,
                        StudentNameDesc = EtmsHelper.GetNameSecrecy(p.StudentName),
                        TagDesc = p.CountFinish >= p.CountLimit ? "砍价成功" : "报名成功"
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<WxMiniActivityDynamicBulletGetPagingOutput>(pagingData.Item2, output));
        }

        private string CheckGroupPurchase(EtActivityMain p, EtActivityRouteItem myActivityRouteItem)
        {
            if (p == null)
            {
                return "活动不存在";
            }
            if (p.ActivityStatus == EmActivityStatus.Unpublished)
            {
                return "活动未发布";
            }
            if (p.ActivityStatus == EmActivityStatus.TakeDown)
            {
                return "活动已下架";
            }
            if (DateTime.Now < p.StartTime)
            {
                return "活动未开始";
            }
            if (DateTime.Now >= p.EndTime.Value)
            {
                return "活动已结束";
            }
            if (p.FinishCount >= p.MaxCount)
            {
                return "活动数量不足，无法参与";
            }
            if (myActivityRouteItem != null)
            {
                if (myActivityRouteItem.IsTeamLeader)
                {
                    return "您已开团，请勿重复操作";
                }
                return "您已参团，无法执行此操作";
            }
            return string.Empty;
        }

        private async Task<EtStudent> GetStudent(string phone, string name)
        {
            var allStudents = await _studentDAL.GetStudentsByPhoneMini(phone);
            if (allStudents == null || allStudents.Count == 0)
            {
                return null;
            }
            if (allStudents.Count == 1)
            {
                return allStudents[0];
            }
            var sameName = allStudents.Where(p => p.Name == name).FirstOrDefault();
            if (sameName != null)
            {
                return sameName;
            }
            return allStudents[0];
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseStartCheck(WxMiniGroupPurchaseStartCheckRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(p.Id, request.MiniPgmUserId);
            var checkMsg = CheckGroupPurchase(p, myActivityRouteItem);
            if (!string.IsNullOrEmpty(checkMsg))
            {
                return ResponseBase.CommonError(checkMsg);
            }
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseStartGo(WxMiniGroupPurchaseStartGoRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var myUser = await _sysWechatMiniPgmUserDAL.GetWechatMiniPgmUser(request.MiniPgmUserId);
            if (myUser == null)
            {
                return ResponseBase.CommonError("用户不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(p.Id, request.MiniPgmUserId);
            var checkMsg = CheckGroupPurchase(p, myActivityRouteItem);
            if (!string.IsNullOrEmpty(checkMsg))
            {
                return ResponseBase.CommonError(checkMsg);
            }
            var lockKey = new ActivityGoToken(request.MiniPgmUserId);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await WxMiniGroupPurchaseStartGoProcess(request, myTenant, p, myUser);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"【WxMiniGroupPurchaseStartGo】出错:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                    throw;
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("操作过于频繁，请稍后再试...");
        }

        private async Task<ResponseBase> WxMiniGroupPurchaseStartGoProcess(WxMiniGroupPurchaseStartGoRequest request,
           SysTenant sysTenant, EtActivityMain p, SysWechatMiniPgmUser user)
        {
            SysTenantSuixingAccount tenantSuixingAccount = null;
            if (p.IsOpenPay)
            {
                if (sysTenant.PayUnionType == EmPayUnionType.NotApplied)
                {
                    return ResponseBase.CommonError("机构未开通支付账户,操作失败");
                }
                tenantSuixingAccount = await _sysTenantSuixingAccountDAL.GetTenantSuixingAccount(sysTenant.Id);
                if (tenantSuixingAccount == null)
                {
                    return ResponseBase.CommonError("机构支付账户异常，操作失败");
                }
            }
            var student = await this.GetStudent(request.StudentPhone, request.StudentName);
            var ruleContent = JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(p.RuleContent);
            var routeStatus = EmActivityRouteStatus.Normal;
            if (p.IsOpenPay)
            {
                routeStatus = EmActivityRouteStatus.Invalid;
            }
            var now = DateTime.Now;
            var countLimit = ruleContent.Item[0].LimitCount;
            var payValue = ComBusiness5.GetActivityPayInfo2(p, ruleContent);
            var myRoute = new EtActivityRoute()
            {
                ActivityId = p.Id,
                ActivityCoverImage = p.CoverImage,
                ActivityEndTime = p.EndTime.Value,
                ActivityName = p.Name,
                ActivityOriginalPrice = p.OriginalPrice,
                ActivityRuleEx1 = p.RuleEx1,
                ActivityRuleEx2 = p.RuleEx2,
                ActivityRuleItemContent = p.RuleContent,
                IsDeleted = EmIsDeleted.Normal,
                ActivityScenetype = p.Scenetype,
                ActivityStartTime = p.StartTime,
                ActivityTenantName = p.TenantName,
                ActivityTitle = p.Title,
                ActivityType = p.ActivityType,
                ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                ScenetypeStyleClass = p.ScenetypeStyleClass,
                TenantId = p.TenantId,
                Tag = string.Empty,
                StudentFieldValue1 = request.StudentFieldValue1,
                StudentFieldValue2 = request.StudentFieldValue2,
                StudentName = request.StudentName,
                StudentPhone = request.StudentPhone,
                AvatarUrl = user.AvatarUrl,
                CountFinish = 1,
                CountLimit = countLimit,
                CreateTime = now,
                Unionid = user.Unionid,
                NickName = user.NickName,
                OpenId = user.OpenId,
                MiniPgmUserId = request.MiniPgmUserId,
                IsNeedPay = p.IsOpenPay,
                PayStatus = EmActivityRoutePayStatus.Unpaid,
                PaySum = payValue,
                PayFinishTime = null,
                RouteStatus = routeStatus,
                StudentId = student?.Id,
                ShareQRCode = string.Empty,
                PayMno = tenantSuixingAccount.Mno
            };
            await _activityRouteDAL.AddActivityRoute(myRoute);
            var myRouteItem = new EtActivityRouteItem()
            {
                ActivityRouteId = myRoute.Id,
                ActivityTypeStyleClass = myRoute.ActivityTypeStyleClass,
                ActivityType = myRoute.ActivityType,
                ActivityTitle = myRoute.ActivityTitle,
                ActivityTenantName = myRoute.ActivityTenantName,
                ActivityCoverImage = myRoute.ActivityCoverImage,
                ActivityEndTime = myRoute.ActivityEndTime,
                ActivityStartTime = myRoute.ActivityStartTime,
                ActivityId = myRoute.ActivityId,
                ActivityName = myRoute.ActivityName,
                ActivityScenetype = myRoute.ActivityScenetype,
                ActivityOriginalPrice = myRoute.ActivityOriginalPrice,
                ActivityRuleItemContent = myRoute.ActivityRuleItemContent,
                ActivityRuleEx2 = myRoute.ActivityRuleEx2,
                ActivityRuleEx1 = myRoute.ActivityRuleEx1,
                AvatarUrl = myRoute.AvatarUrl,
                CreateTime = myRoute.CreateTime,
                IsDeleted = myRoute.IsDeleted,
                IsNeedPay = myRoute.IsNeedPay,
                IsTeamLeader = true,
                MiniPgmUserId = myRoute.MiniPgmUserId,
                NickName = myRoute.NickName,
                OpenId = myRoute.OpenId,
                PayFinishTime = myRoute.PayFinishTime,
                PayStatus = myRoute.PayStatus,
                PaySum = myRoute.PaySum,
                RouteStatus = myRoute.RouteStatus,
                ScenetypeStyleClass = myRoute.ScenetypeStyleClass,
                StudentFieldValue1 = myRoute.StudentFieldValue1,
                StudentFieldValue2 = myRoute.StudentFieldValue2,
                StudentId = myRoute.StudentId,
                StudentName = myRoute.StudentName,
                StudentPhone = myRoute.StudentPhone,
                Tag = myRoute.Tag,
                TenantId = myRoute.TenantId,
                Unionid = myRoute.Unionid,
                ShareQRCode = string.Empty,
                PayMno = tenantSuixingAccount.Mno
            };
            await _activityRouteDAL.AddActivityRouteItem(myRouteItem);

            _eventPublisher.Publish(new CalculateActivityRouteIInfoEvent(request.TenantId)
            {
                MyActivityRouteItem = myRouteItem
            });
            var output = new WxMiniGroupPurchaseStartGoProcessOutput()
            {
                IsMustPay = false,
                ActivityRouteItemId = myRouteItem.Id
            };
            if (p.IsOpenPay) //如果需要支付，则支付完成后才算成功
            {
                var no = OrderNumberLib.SuixingPayOrder();
                output.IsMustPay = true;
                var res = _paySuixingService.JsapiScanMiniProgram(new JsapiScanMiniProgramReq()
                {
                    mno = tenantSuixingAccount.Mno,
                    amt = myRouteItem.PaySum,
                    extend = $"{myRouteItem.TenantId}_{myRouteItem.Id}",
                    openid = myRouteItem.OpenId,
                    ordNo = no,
                    subject = "参加活动",
                    notifyUrl = SysWebApiAddressConfig.SuixingPayCallbackUrl
                });
                if (res == null)
                {
                    return ResponseBase.CommonError("生成预支付订单失败");
                }
                await _activityRouteDAL.UpdateActivityRoutePayOrder(myRoute.Id, no, res.uuid);
                await _activityRouteDAL.UpdateActivityRouteItemPayOrder(myRouteItem.Id, no, res.uuid);
                output.PayInfo = new WxMiniGroupPurchasePayInfo()
                {
                    routeItemId = myRouteItem.Id,
                    orderNo = no,
                    uuid = res.uuid,
                    appId = res.payAppId,
                    nonceStr = res.paynonceStr,
                    package_str = res.payPackage,
                    paySign = res.paySign,
                    signType = res.paySignType,
                    timeStamp = res.payTimeStamp,
                    token_id = string.Empty
                };
            }
            else
            {
                _eventPublisher.Publish(new SyncActivityRouteFinishCountEvent(request.TenantId)
                {
                    ActivityId = p.Id,
                    CountLimit = myRoute.CountLimit,
                    ActivityRouteId = myRoute.Id,
                    ActivityRouteItemId = myRouteItem.Id,
                    ActivityType = p.ActivityType,
                    RuleContent = p.RuleContent
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseJoinCheck(WxMiniGroupPurchaseJoinCheckRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(p.Id, request.MiniPgmUserId);
            var checkMsg = CheckGroupPurchase(p, myActivityRouteItem);
            if (!string.IsNullOrEmpty(checkMsg))
            {
                return ResponseBase.CommonError(checkMsg);
            }
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniGroupPurchaseJoin(WxMiniGroupPurchaseJoinRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var myActivityRoute = await _activityRouteDAL.GetActivityRoute(request.ActivityRouteId);
            if (myActivityRoute == null)
            {
                return ResponseBase.CommonError("开团记录不存在");
            }
            var myUser = await _sysWechatMiniPgmUserDAL.GetWechatMiniPgmUser(request.MiniPgmUserId);
            if (myUser == null)
            {
                return ResponseBase.CommonError("用户不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(myActivityRoute.ActivityId);
            var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(p.Id, request.MiniPgmUserId);
            var checkMsg = CheckGroupPurchase(p, myActivityRouteItem);
            if (!string.IsNullOrEmpty(checkMsg))
            {
                return ResponseBase.CommonError(checkMsg);
            }
            var lockKey = new ActivityGoToken(request.MiniPgmUserId);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await WxMiniGroupPurchaseJoinProcess(request, myTenant, p, myUser, myActivityRoute);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"【WxMiniGroupPurchaseJoin】出错:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                    throw;
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("操作过于频繁，请稍后再试...");
        }

        private async Task<ResponseBase> WxMiniGroupPurchaseJoinProcess(WxMiniGroupPurchaseJoinRequest request,
            SysTenant sysTenant, EtActivityMain p, SysWechatMiniPgmUser user, EtActivityRoute myActivityRoute)
        {
            SysTenantSuixingAccount tenantSuixingAccount = null;
            if (p.IsOpenPay)
            {
                if (sysTenant.PayUnionType == EmPayUnionType.NotApplied)
                {
                    return ResponseBase.CommonError("机构未开通支付账户,操作失败");
                }
                tenantSuixingAccount = await _sysTenantSuixingAccountDAL.GetTenantSuixingAccount(sysTenant.Id);
                if (tenantSuixingAccount == null)
                {
                    return ResponseBase.CommonError("机构支付账户异常，操作失败");
                }
            }
            var student = await this.GetStudent(request.StudentPhone, request.StudentName);
            var ruleContent = JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(p.RuleContent);
            var routeStatus = EmActivityRouteStatus.Normal;
            if (p.IsOpenPay)
            {
                routeStatus = EmActivityRouteStatus.Invalid;
            }
            var now = DateTime.Now;
            var payValue = ComBusiness5.GetActivityPayInfo2(p, ruleContent);
            var myRouteItem = new EtActivityRouteItem()
            {
                ActivityRouteId = myActivityRoute.Id,
                ActivityTypeStyleClass = myActivityRoute.ActivityTypeStyleClass,
                ActivityType = myActivityRoute.ActivityType,
                ActivityTitle = myActivityRoute.ActivityTitle,
                ActivityTenantName = myActivityRoute.ActivityTenantName,
                ActivityCoverImage = myActivityRoute.ActivityCoverImage,
                ActivityEndTime = myActivityRoute.ActivityEndTime,
                ActivityStartTime = myActivityRoute.ActivityStartTime,
                ActivityId = myActivityRoute.ActivityId,
                ActivityName = myActivityRoute.ActivityName,
                ActivityScenetype = myActivityRoute.ActivityScenetype,
                ActivityOriginalPrice = myActivityRoute.ActivityOriginalPrice,
                ActivityRuleItemContent = myActivityRoute.ActivityRuleItemContent,
                ActivityRuleEx2 = myActivityRoute.ActivityRuleEx2,
                ActivityRuleEx1 = myActivityRoute.ActivityRuleEx1,
                ScenetypeStyleClass = myActivityRoute.ScenetypeStyleClass,
                AvatarUrl = user.AvatarUrl,
                NickName = user.NickName,
                OpenId = user.OpenId,
                MiniPgmUserId = user.Id,
                Unionid = user.Unionid,
                CreateTime = now,
                IsDeleted = myActivityRoute.IsDeleted,
                IsNeedPay = p.IsOpenPay,
                IsTeamLeader = false,
                PayFinishTime = null,
                PayStatus = EmActivityRoutePayStatus.Unpaid,
                PaySum = payValue,
                RouteStatus = routeStatus,
                StudentFieldValue1 = request.StudentFieldValue1,
                StudentFieldValue2 = request.StudentFieldValue2,
                StudentId = student?.Id,
                StudentName = request.StudentName,
                StudentPhone = request.StudentPhone,
                Tag = string.Empty,
                TenantId = myActivityRoute.TenantId,
                ShareQRCode = string.Empty,
                PayMno = tenantSuixingAccount.Mno
            };
            await _activityRouteDAL.AddActivityRouteItem(myRouteItem);

            _eventPublisher.Publish(new CalculateActivityRouteIInfoEvent(request.TenantId)
            {
                MyActivityRouteItem = myRouteItem
            });
            var output = new WxMiniGroupPurchaseJoinProcessOutput()
            {
                IsMustPay = false,
                ActivityRouteItemId = myRouteItem.Id
            };
            if (p.IsOpenPay) //如果需要支付，则支付完成后才算成功
            {
                var no = OrderNumberLib.SuixingPayOrder();
                output.IsMustPay = true;
                var res = _paySuixingService.JsapiScanMiniProgram(new JsapiScanMiniProgramReq()
                {
                    mno = tenantSuixingAccount.Mno,
                    amt = myRouteItem.PaySum,
                    extend = $"{myRouteItem.TenantId}_{myRouteItem.Id}",
                    openid = myRouteItem.OpenId,
                    ordNo = no,
                    subject = "参加活动",
                    notifyUrl = SysWebApiAddressConfig.SuixingPayCallbackUrl
                });
                if (res == null)
                {
                    return ResponseBase.CommonError("生成预支付订单失败");
                }
                await _activityRouteDAL.UpdateActivityRouteItemPayOrder(myRouteItem.Id, no, res.uuid);
                output.PayInfo = new WxMiniGroupPurchasePayInfo()
                {
                    routeItemId = myRouteItem.Id,
                    orderNo = no,
                    uuid = res.uuid,
                    appId = res.payAppId,
                    nonceStr = res.paynonceStr,
                    package_str = res.payPackage,
                    paySign = res.paySign,
                    signType = res.paySignType,
                    timeStamp = res.payTimeStamp,
                    token_id = string.Empty
                };
            }
            else
            {
                await _activityRouteDAL.TempAddActivityRouteCount(myActivityRoute.Id);
                _eventPublisher.Publish(new SyncActivityRouteFinishCountEvent(request.TenantId)
                {
                    ActivityId = p.Id,
                    CountLimit = myActivityRoute.CountLimit,
                    ActivityRouteId = myActivityRoute.Id,
                    ActivityRouteItemId = myRouteItem.Id,
                    ActivityType = p.ActivityType,
                    RuleContent = p.RuleContent
                });
            }
            return ResponseBase.Success(output);
        }

        private string CheckHaggling(EtActivityMain p, EtActivityRouteItem myActivityRouteItem)
        {
            if (p == null)
            {
                return "活动不存在";
            }
            if (p.ActivityStatus == EmActivityStatus.Unpublished)
            {
                return "活动未发布";
            }
            if (p.ActivityStatus == EmActivityStatus.TakeDown)
            {
                return "活动已下架";
            }
            if (DateTime.Now < p.StartTime)
            {
                return "活动未开始";
            }
            if (DateTime.Now >= p.EndTime.Value)
            {
                return "活动已结束";
            }
            if (p.FinishCount >= p.MaxCount)
            {
                return "活动数量不足，无法参与";
            }
            if (myActivityRouteItem != null)
            {
                return "您已发起砍价";
            }
            return string.Empty;
        }

        public async Task<ResponseBase> WxMiniHagglingStartCheck(WxMiniHagglingStartCheckRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(p.Id, request.MiniPgmUserId);
            var checkMsg = CheckHaggling(p, myActivityRouteItem);
            if (!string.IsNullOrEmpty(checkMsg))
            {
                return ResponseBase.CommonError(checkMsg);
            }
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> WxMiniHagglingStartGo(WxMiniHagglingStartGoRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var myUser = await _sysWechatMiniPgmUserDAL.GetWechatMiniPgmUser(request.MiniPgmUserId);
            if (myUser == null)
            {
                return ResponseBase.CommonError("用户不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(request.ActivityMainId);
            var myActivityRouteItem = await _activityRouteDAL.GetEtActivityRouteItemByUserId(p.Id, request.MiniPgmUserId);
            var checkMsg = CheckHaggling(p, myActivityRouteItem);
            if (!string.IsNullOrEmpty(checkMsg))
            {
                return ResponseBase.CommonError(checkMsg);
            }
            var lockKey = new ActivityGoToken(request.MiniPgmUserId);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await WxMiniHagglingStartGo(request, myTenant, p, myUser);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"【WxMiniHagglingStartGo】出错:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                    throw;
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("操作过于频繁，请稍后再试...");
        }

        private async Task<ResponseBase> WxMiniHagglingStartGo(WxMiniHagglingStartGoRequest request,
            SysTenant sysTenant, EtActivityMain p, SysWechatMiniPgmUser user)
        {
            SysTenantSuixingAccount tenantSuixingAccount = null;
            if (p.IsOpenPay)
            {
                if (sysTenant.PayUnionType == EmPayUnionType.NotApplied)
                {
                    return ResponseBase.CommonError("机构未开通支付账户,操作失败");
                }
                tenantSuixingAccount = await _sysTenantSuixingAccountDAL.GetTenantSuixingAccount(sysTenant.Id);
                if (tenantSuixingAccount == null)
                {
                    return ResponseBase.CommonError("机构支付账户异常，操作失败");
                }
            }
            var student = await this.GetStudent(request.StudentPhone, request.StudentName);
            var lowPrice = p.RuleEx1.ToDecimal();
            var limitMustCount = p.RuleEx2.ToInt();
            //var myRepeatHaggleHour = p.RuleEx3.ToInt();
            var routeStatus = EmActivityRouteStatus.Normal;
            if (p.IsOpenPay)
            {
                routeStatus = EmActivityRouteStatus.Invalid;
            }
            var now = DateTime.Now;
            var myRoute = new EtActivityRoute()
            {
                ActivityId = p.Id,
                ActivityCoverImage = p.CoverImage,
                ActivityEndTime = p.EndTime.Value,
                ActivityName = p.Name,
                ActivityOriginalPrice = p.OriginalPrice,
                ActivityRuleEx1 = p.RuleEx1,
                ActivityRuleEx2 = p.RuleEx2,
                ActivityRuleItemContent = p.RuleContent,
                IsDeleted = EmIsDeleted.Normal,
                ActivityScenetype = p.Scenetype,
                ActivityStartTime = p.StartTime,
                ActivityTenantName = p.TenantName,
                ActivityTitle = p.Title,
                ActivityType = p.ActivityType,
                ActivityTypeStyleClass = p.ActivityTypeStyleClass,
                ScenetypeStyleClass = p.ScenetypeStyleClass,
                TenantId = p.TenantId,
                Tag = string.Empty,
                StudentFieldValue1 = request.StudentFieldValue1,
                StudentFieldValue2 = request.StudentFieldValue2,
                StudentName = request.StudentName,
                StudentPhone = request.StudentPhone,
                AvatarUrl = user.AvatarUrl,
                CountFinish = 1,
                CountLimit = limitMustCount,
                CreateTime = now,
                Unionid = user.Unionid,
                NickName = user.NickName,
                OpenId = user.OpenId,
                MiniPgmUserId = request.MiniPgmUserId,
                IsNeedPay = p.IsOpenPay,
                PayStatus = EmActivityRoutePayStatus.Unpaid,
                PaySum = lowPrice,
                PayFinishTime = null,
                RouteStatus = routeStatus,
                StudentId = student?.Id,
                ShareQRCode = string.Empty,
                PayMno = tenantSuixingAccount.Mno
            };
            await _activityRouteDAL.AddActivityRoute(myRoute);
            var myRouteItem = new EtActivityRouteItem()
            {
                ActivityRouteId = myRoute.Id,
                ActivityTypeStyleClass = myRoute.ActivityTypeStyleClass,
                ActivityType = myRoute.ActivityType,
                ActivityTitle = myRoute.ActivityTitle,
                ActivityTenantName = myRoute.ActivityTenantName,
                ActivityCoverImage = myRoute.ActivityCoverImage,
                ActivityEndTime = myRoute.ActivityEndTime,
                ActivityStartTime = myRoute.ActivityStartTime,
                ActivityId = myRoute.ActivityId,
                ActivityName = myRoute.ActivityName,
                ActivityScenetype = myRoute.ActivityScenetype,
                ActivityOriginalPrice = myRoute.ActivityOriginalPrice,
                ActivityRuleItemContent = myRoute.ActivityRuleItemContent,
                ActivityRuleEx2 = myRoute.ActivityRuleEx2,
                ActivityRuleEx1 = myRoute.ActivityRuleEx1,
                AvatarUrl = myRoute.AvatarUrl,
                CreateTime = myRoute.CreateTime,
                IsDeleted = myRoute.IsDeleted,
                IsNeedPay = myRoute.IsNeedPay,
                IsTeamLeader = true,
                MiniPgmUserId = myRoute.MiniPgmUserId,
                NickName = myRoute.NickName,
                OpenId = myRoute.OpenId,
                PayFinishTime = myRoute.PayFinishTime,
                PayStatus = myRoute.PayStatus,
                PaySum = myRoute.PaySum,
                RouteStatus = myRoute.RouteStatus,
                ScenetypeStyleClass = myRoute.ScenetypeStyleClass,
                StudentFieldValue1 = myRoute.StudentFieldValue1,
                StudentFieldValue2 = myRoute.StudentFieldValue2,
                StudentId = myRoute.StudentId,
                StudentName = myRoute.StudentName,
                StudentPhone = myRoute.StudentPhone,
                Tag = myRoute.Tag,
                TenantId = myRoute.TenantId,
                Unionid = myRoute.Unionid,
                ShareQRCode = string.Empty,
                PayMno = tenantSuixingAccount.Mno
            };
            await _activityRouteDAL.AddActivityRouteItem(myRouteItem);

            _eventPublisher.Publish(new CalculateActivityRouteIInfoEvent(request.TenantId)
            {
                MyActivityRouteItem = myRouteItem
            });
            var output = new WxMiniHagglingStartGoOutput()
            {
                IsMustPay = false,
                ActivityRouteItemId = myRouteItem.Id
            };
            if (p.IsOpenPay) //如果需要支付，则支付完成后才算成功
            {
                var no = OrderNumberLib.SuixingPayOrder();
                output.IsMustPay = true;
                var res = _paySuixingService.JsapiScanMiniProgram(new JsapiScanMiniProgramReq()
                {
                    mno = tenantSuixingAccount.Mno,
                    amt = myRouteItem.PaySum,
                    extend = $"{myRouteItem.TenantId}_{myRouteItem.Id}",
                    openid = myRouteItem.OpenId,
                    ordNo = no,
                    subject = "参加活动",
                    notifyUrl = SysWebApiAddressConfig.SuixingPayCallbackUrl
                });
                if (res == null)
                {
                    return ResponseBase.CommonError("生成预支付订单失败");
                }
                await _activityRouteDAL.UpdateActivityRoutePayOrder(myRoute.Id, no, res.uuid);
                await _activityRouteDAL.UpdateActivityRouteItemPayOrder(myRouteItem.Id, no, res.uuid);
                output.PayInfo = new WxMiniGroupPurchasePayInfo()
                {
                    routeItemId = myRouteItem.Id,
                    orderNo = no,
                    uuid = res.uuid,
                    appId = res.payAppId,
                    nonceStr = res.paynonceStr,
                    package_str = res.payPackage,
                    paySign = res.paySign,
                    signType = res.paySignType,
                    timeStamp = res.payTimeStamp,
                    token_id = string.Empty
                };
            }
            else
            {
                _eventPublisher.Publish(new SyncActivityRouteFinishCountEvent(request.TenantId)
                {
                    ActivityId = p.Id,
                    CountLimit = myRoute.CountLimit,
                    ActivityRouteId = myRoute.Id,
                    ActivityRouteItemId = myRouteItem.Id,
                    ActivityType = p.ActivityType,
                    RuleContent = p.RuleContent
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> WxMiniHagglingAssistGo(WxMiniHagglingAssistGoRequest request)
        {
            this.InitTenantId(request.TenantId);
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var myUser = await _sysWechatMiniPgmUserDAL.GetWechatMiniPgmUser(request.MiniPgmUserId);
            if (myUser == null)
            {
                return ResponseBase.CommonError("用户不存在");
            }
            var myActivityRouteItem = await _activityRouteDAL.GetActivityRouteItem(request.ActivityRouteItemId);
            if (myActivityRouteItem == null)
            {
                return ResponseBase.CommonError("砍价记录不存在");
            }
            var myActivityRoute = await _activityRouteDAL.GetActivityRoute(myActivityRouteItem.ActivityRouteId);
            if (myActivityRoute == null)
            {
                return ResponseBase.CommonError("砍价记录不存在");
            }
            var p = await _activityMainDAL.GetActivityMain(myActivityRouteItem.ActivityId);
            if (p == null)
            {
                return ResponseBase.CommonError("活动不存在");
            }
            if (p.ActivityStatus == EmActivityStatus.Unpublished)
            {
                return ResponseBase.CommonError("活动未发布");
            }
            if (p.ActivityStatus == EmActivityStatus.TakeDown)
            {
                return ResponseBase.CommonError("活动已下架");
            }
            if (DateTime.Now < p.StartTime)
            {
                return ResponseBase.CommonError("活动未开始");
            }
            if (DateTime.Now >= p.EndTime.Value)
            {
                return ResponseBase.CommonError("活动已结束");
            }
            var lockKey = new ActivityGoToken(request.MiniPgmUserId);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await WxMiniHagglingAssistGoProcess(request, myTenant, p, myActivityRouteItem, myActivityRoute, myUser);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"【WxMiniGroupPurchaseStartGo】出错:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                    throw;
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("操作过于频繁，请稍后再试...");
        }

        private async Task<ResponseBase> WxMiniHagglingAssistGoProcess(WxMiniHagglingAssistGoRequest request,
            SysTenant sysTenant, EtActivityMain p, EtActivityRouteItem myActivityRouteItem, EtActivityRoute myActivityRoute, SysWechatMiniPgmUser user)
        {
            var now = DateTime.Now;
            var haggleLog = await _activityRouteDAL.GetActivityHaggleLog(myActivityRouteItem.ActivityId, myActivityRouteItem.ActivityRouteId, request.MiniPgmUserId);
            if (haggleLog != null)
            {
                if (haggleLog.MiniPgmUserId == myActivityRouteItem.MiniPgmUserId) //给自己砍价
                {
                    var myRepeatHaggleHour = p.RuleEx3.ToInt();
                    if (myRepeatHaggleHour == 0)
                    {
                        return ResponseBase.CommonError("请勿重复操作");
                    }
                    else
                    {
                        var vaildTime = haggleLog.CreateTime.AddHours(myRepeatHaggleHour);
                        if (now < vaildTime)
                        {
                            return ResponseBase.CommonError("距离上一次砍价间隔不足，请稍后再试...");
                        }
                    }
                }
                else
                {
                    return ResponseBase.CommonError("请勿重复操作");
                }
            }
            await _activityRouteDAL.AddActivityHaggleLog(new EtActivityHaggleLog()
            {
                ActivityId = p.Id,
                ActivityRouteId = myActivityRouteItem.ActivityRouteId,
                AvatarUrl = user.AvatarUrl,
                MiniPgmUserId = user.Id,
                OpenId = user.OpenId,
                Unionid = user.Unionid,
                NickName = user.NickName,
                IsDeleted = EmIsDeleted.Normal,
                CreateDate = now.Date,
                CreateTime = now,
                TenantId = myActivityRouteItem.TenantId
            });
            _eventPublisher.Publish(new SyncActivityRouteFinishCountEvent(request.TenantId)
            {
                ActivityId = p.Id,
                CountLimit = myActivityRoute.CountLimit,
                ActivityRouteId = myActivityRouteItem.ActivityRouteId,
                ActivityRouteItemId = myActivityRouteItem.Id,
                ActivityType = p.ActivityType,
                RuleContent = p.RuleContent
            });
            return ResponseBase.Success();
        }

        public ResponseBase WxMiniActivityCall(WxMiniActivityCallRequest request)
        {
            _eventPublisher.Publish(new SyncActivityBehaviorCountEvent(request.TenantId)
            {
                ActivityId = request.ActivityMainId,
                MiniPgmUserId = request.MiniPgmUserId,
                BehaviorType = ActivityBehaviorType.Access
            });
            return ResponseBase.Success();
        }

        public ResponseBase WxMiniActivityShare(WxMiniActivityShareRequest request)
        {
            _eventPublisher.Publish(new SyncActivityBehaviorCountEvent(request.TenantId)
            {
                ActivityId = request.ActivityMainId,
                MiniPgmUserId = request.MiniPgmUserId,
                BehaviorType = ActivityBehaviorType.Retweet
            });
            return ResponseBase.Success();
        }

        public ResponseBase WxMiniPaySuccess(WxMiniPaySuccessRequest request)
        {
            _eventPublisher.Publish(new SuixingPayCallbackEvent(request.TenantId)
            {
                ActivityRouteItemId = request.ActivityRouteItemId,
                PayTime = DateTime.Now
            });
            return ResponseBase.Success();
        }
    }
}
