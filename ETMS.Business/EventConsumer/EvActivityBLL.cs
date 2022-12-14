using ETMS.Business.Common;
using ETMS.Business.WxCore;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Temp.Request;
using ETMS.Entity.View.Activity;
using ETMS.Event.DataContract.Activity;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IDataAccess.Activity;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.Lcs;
using ETMS.IEventProvider;
using ETMS.Pay.Suixing;
using ETMS.Pay.Suixing.Utility.Dto;
using ETMS.Pay.Suixing.Utility.ExternalDto.Request;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvActivityBLL : MiniProgramAccessBll, IEvActivityBLL
    {
        private readonly IActivityMainDAL _activityMainDAL;

        private readonly IActivityRouteDAL _activityRouteDAL;

        private readonly IActivityVisitorDAL _activityVisitorDAL;

        private readonly ISysActivityRouteItemDAL _sysActivityRouteItemDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly ITenantLcsPayLogDAL _tenantLcsPayLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysTenantSuixingAccountDAL _sysTenantSuixingAccountDAL;

        private readonly ITenantConfig2DAL _tenantConfig2DAL;

        private readonly IPaySuixingService _paySuixingService;

        public EvActivityBLL(IAppConfigurtaionServices appConfigurtaionServices, IActivityMainDAL activityMainDAL, IActivityRouteDAL activityRouteDAL,
           IActivityVisitorDAL activityVisitorDAL, ISysActivityRouteItemDAL sysActivityRouteItemDAL, IEventPublisher eventPublisher,
           ITenantLcsPayLogDAL tenantLcsPayLogDAL, ISysTenantDAL sysTenantDAL, ISysTenantSuixingAccountDAL sysTenantSuixingAccountDAL,
           ITenantConfig2DAL tenantConfig2DAL, IPaySuixingService paySuixingService)
            : base(appConfigurtaionServices)
        {
            this._activityMainDAL = activityMainDAL;
            this._activityRouteDAL = activityRouteDAL;
            this._activityVisitorDAL = activityVisitorDAL;
            this._sysActivityRouteItemDAL = sysActivityRouteItemDAL;
            this._eventPublisher = eventPublisher;
            this._tenantLcsPayLogDAL = tenantLcsPayLogDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysTenantSuixingAccountDAL = sysTenantSuixingAccountDAL;
            this._tenantConfig2DAL = tenantConfig2DAL;
            this._paySuixingService = paySuixingService;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _activityMainDAL, _activityRouteDAL, _activityVisitorDAL,
                _tenantLcsPayLogDAL, _tenantConfig2DAL);
        }

        public async Task SyncActivityBehaviorCountConsumerEvent(SyncActivityBehaviorCountEvent request)
        {
            if (request.BehaviorType == ActivityBehaviorType.Retweet)
            {
                await _activityMainDAL.AddBehaviorCount(request.ActivityId, 0, 0, 1, 0);
            }
            else
            {
                var visitorLog = await _activityVisitorDAL.GetActivityVisitor(request.ActivityId, request.MiniPgmUserId);
                if (visitorLog != null)
                {
                    await _activityMainDAL.AddBehaviorCount(request.ActivityId, 1, 0, 0, 1);
                }
                else
                {
                    await _activityMainDAL.AddBehaviorCount(request.ActivityId, 1, 1, 0, 1);
                    await _activityVisitorDAL.AddActivityVisitor(new EtActivityVisitor()
                    {
                        ActivityId = request.ActivityId,
                        CreateTime = DateTime.Now,
                        IsDeleted = EmIsDeleted.Normal,
                        MiniPgmUserId = request.MiniPgmUserId,
                        OpenId = string.Empty,
                        TenantId = request.TenantId,
                        Unionid = string.Empty
                    });
                }
            }
        }

        public async Task SyncActivityEffectCountConsumerEvent(SyncActivityEffectCountEvent request)
        {
            var joinCount = await _activityRouteDAL.GetJoinCount(request.ActivityId, request.ActivityType);
            var routeCount = await _activityRouteDAL.GetRouteCount(request.ActivityId, request.ActivityType);
            var finishCount = await _activityRouteDAL.GetFinishCount(request.ActivityId, request.ActivityType);
            var finishFullCount = 0;
            if (request.ActivityType == EmActivityType.GroupPurchase)
            {
                finishFullCount = await _activityRouteDAL.GetFinishFullCount(request.ActivityId);
            }
            await _activityMainDAL.SetEffectCount(request.ActivityId, joinCount, routeCount, finishCount, finishFullCount);
        }

        /// <summary>
        /// 更新活动参与记录状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SyncActivityRouteFinishCountConsumerEvent(SyncActivityRouteFinishCountEvent request)
        {
            var countFinish = await _activityRouteDAL.GetActivityRouteFinishCount(request.ActivityRouteId, request.ActivityType);
            var status = EmSysActivityRouteItemStatus.Going;
            if (request.ActivityType == EmActivityType.GroupPurchase)
            {
                var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(request.RuleContent);
                var minCount = ruleContent.Item.First().LimitCount;
                var maxCount = ruleContent.Item.Last().LimitCount;
                status = ComBusiness5.GetSysActivityRouteItemStatus(minCount, maxCount, countFinish);
            }
            else
            {
                if (countFinish >= request.CountLimit)
                {
                    status = EmSysActivityRouteItemStatus.FinishFull;
                }
            }
            await _activityRouteDAL.SetFinishCountAndStatus(request.ActivityRouteId, countFinish, status);
            await _sysActivityRouteItemDAL.UpdateActivityRouteItemInfo(request.TenantId, request.ActivityId, request.ActivityRouteId, status, countFinish);
            if (status != EmSysActivityRouteItemStatus.Going)
            {
                await _activityRouteDAL.SetActivityRouteItemStatus(request.ActivityRouteId, status);
            }
            _eventPublisher.Publish(new SyncSysActivityRouteItemEvent(request.TenantId)
            {
                ActivityRouteItemId = request.ActivityRouteItemId,
            });
            _eventPublisher.Publish(new SyncActivityEffectCountEvent(request.TenantId)
            {
                ActivityId = request.ActivityId,
                ActivityType = request.ActivityType
            });
        }

        public async Task SyncActivityBascInfoConsumerEvent(SyncActivityBascInfoEvent request)
        {
            await _activityRouteDAL.SyncActivityBascInfo(request.NewActivityMain);
            await _sysActivityRouteItemDAL.SyncActivityBascInfo(request.NewActivityMain);
        }

        public async Task CalculateActivityRouteIInfoConsumerEvent(CalculateActivityRouteIInfoEvent request)
        {
            var id = request.MyActivityRouteItem.Id;
            var key = $"qr_route_item_{request.TenantId}_{id}.png";
            var scene = $"{request.TenantId}_{request.MyActivityRouteItem.ActivityId}_{id}";
            var routeShareQRCodeKey = await GenerateQrCode(request.TenantId,
                AliyunOssFileTypeEnum.ActivityRouteQrCode, key, MiniProgramPathConfig.GetMiniProgramPath(request.MyActivityRouteItem.ActivityType), scene);
            await _activityRouteDAL.UpdateActivityRouteItemShareQRCodeInfo(id, routeShareQRCodeKey);
            if (request.MyActivityRouteItem.IsTeamLeader)
            {
                await _activityRouteDAL.UpdateActivityRouteShareQRCodeInfo(request.MyActivityRouteItem.ActivityRouteId, routeShareQRCodeKey);
            }
        }

        public async Task SyncSysActivityRouteItemConsumerEvent(SyncSysActivityRouteItemEvent request)
        {
            var item = await _activityRouteDAL.GetActivityRouteItem(request.ActivityRouteItemId);
            if (item == null)
            {
                await _sysActivityRouteItemDAL.DelSysActivityRouteItemByRouteItemId(request.TenantId, request.ActivityRouteItemId);
                return;
            }
            var log = await _sysActivityRouteItemDAL.GetSysActivityRouteItem(request.TenantId, request.ActivityRouteItemId);
            if (log == null)
            {
                var myActivityRoute = await _activityRouteDAL.GetActivityRoute(item.ActivityRouteId);
                var countLimit = myActivityRoute.CountLimit;
                var countLimitMax = myActivityRoute.CountLimit;
                var countFinish = myActivityRoute.CountFinish;
                var p = await _activityMainDAL.GetActivityMain(myActivityRoute.ActivityId);
                if (myActivityRoute.ActivityType == EmActivityType.GroupPurchase)
                {
                    var ruleContent = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityOfGroupPurchaseRuleContentView>(p.RuleContent);
                    countLimitMax = ruleContent.Item.Last().LimitCount;
                }
                await _sysActivityRouteItemDAL.AddSysActivityRouteItem(new SysActivityRouteItem()
                {
                    TenantId = item.TenantId,
                    ActivityId = item.ActivityId,
                    ActivityRouteId = item.ActivityRouteId,
                    EtActivityRouteItemId = item.Id,
                    IsTeamLeader = item.IsTeamLeader,
                    IsDeleted = item.IsDeleted,
                    MiniPgmUserId = item.MiniPgmUserId,
                    NickName = item.NickName,
                    OpenId = item.OpenId,
                    PayFinishTime = item.PayFinishTime,
                    PaySum = item.PaySum,
                    ScenetypeStyleClass = item.ScenetypeStyleClass,
                    StudentFieldValue1 = item.StudentFieldValue1,
                    StudentFieldValue2 = item.StudentFieldValue2,
                    StudentId = item.StudentId,
                    StudentName = item.StudentName,
                    StudentPhone = item.StudentPhone,
                    Unionid = item.Unionid,
                    ActivityCoverImage = item.ActivityCoverImage,
                    ActivityEndTime = item.ActivityEndTime,
                    ActivityName = item.ActivityName,
                    ActivityOriginalPrice = item.ActivityOriginalPrice,
                    ActivityRuleEx1 = item.ActivityRuleEx1,
                    ActivityRuleEx2 = item.ActivityRuleEx2,
                    ActivityRuleItemContent = item.ActivityRuleItemContent,
                    ActivityScenetype = item.ActivityScenetype,
                    ActivityStartTime = item.ActivityStartTime,
                    ActivityTenantName = item.ActivityTenantName,
                    ActivityTitle = item.ActivityTitle,
                    ActivityType = item.ActivityType,
                    ActivityTypeStyleClass = item.ActivityTypeStyleClass,
                    AvatarUrl = item.AvatarUrl,
                    CreateTime = item.CreateTime,
                    Status = item.Status,
                    PayStatus = item.PayStatus,
                    CountLimit = countLimit,
                    CountLimitMax = countLimitMax,
                    CountFinish = countFinish,
                    ActivityIsOpenPay = p.IsOpenPay,
                    ActivityPayType = p.PayType
                });
            }
            else
            {
                log.MiniPgmUserId = item.MiniPgmUserId;
                log.NickName = item.NickName;
                log.OpenId = item.OpenId;
                log.PayFinishTime = item.PayFinishTime;
                log.PaySum = item.PaySum;
                log.ScenetypeStyleClass = item.ScenetypeStyleClass;
                log.StudentFieldValue1 = item.StudentFieldValue1;
                log.StudentFieldValue2 = item.StudentFieldValue2;
                log.StudentId = item.StudentId;
                log.StudentName = item.StudentName;
                log.StudentPhone = item.StudentPhone;
                log.Unionid = item.Unionid;
                log.ActivityCoverImage = item.ActivityCoverImage;
                log.ActivityEndTime = item.ActivityEndTime;
                log.ActivityName = item.ActivityName;
                log.ActivityOriginalPrice = item.ActivityOriginalPrice;
                log.ActivityRuleEx1 = item.ActivityRuleEx1;
                log.ActivityRuleEx2 = item.ActivityRuleEx2;
                log.ActivityRuleItemContent = item.ActivityRuleItemContent;
                log.ActivityScenetype = item.ActivityScenetype;
                log.ActivityStartTime = item.ActivityStartTime;
                log.ActivityTenantName = item.ActivityTenantName;
                log.ActivityTitle = item.ActivityTitle;
                log.ActivityType = item.ActivityType;
                log.ActivityTypeStyleClass = item.ActivityTypeStyleClass;
                log.AvatarUrl = item.AvatarUrl;
                log.Status = item.Status;
                log.PayStatus = item.PayStatus;
                await _sysActivityRouteItemDAL.EdiSysActivityRouteItem(log);
            }
        }

        public async Task SuixingPayCallbackConsumerEvent(SuixingPayCallbackEvent request)
        {
            var payTime = request.PayTime;
            var myRouteItemId = request.ActivityRouteItemId;
            var myActivityRouteItem = await _activityRouteDAL.GetActivityRouteItemTemp(myRouteItemId);
            if (myActivityRouteItem.PayStatus == EmActivityRoutePayStatus.Paid) //重复操作
            {
                return;
            }
            await _activityRouteDAL.UpdateActivityRouteItemAboutPayFinishTemp(myRouteItemId, payTime);
            if (myActivityRouteItem.IsTeamLeader)
            {
                await _activityRouteDAL.UpdateActivityRouteAboutPayFinishTemp(myActivityRouteItem.ActivityRouteId, payTime);
            }
            var myActivityRoute = await _activityRouteDAL.GetActivityRouteTemp(myActivityRouteItem.ActivityRouteId);
            var myActivity = await _activityMainDAL.GetActivityMain(myActivityRoute.ActivityId);
            _eventPublisher.Publish(new SyncActivityRouteFinishCountEvent(myActivityRouteItem.TenantId)
            {
                ActivityId = myActivityRouteItem.ActivityId,
                CountLimit = myActivityRoute.CountLimit,
                ActivityRouteId = myActivityRoute.Id,
                ActivityRouteItemId = myActivityRouteItem.Id,
                ActivityType = myActivityRouteItem.ActivityType,
                RuleContent = myActivity.RuleContent
            });

            //聚会支付记录
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            var myTenantSuixingAccount = await _sysTenantSuixingAccountDAL.GetTenantSuixingAccount(request.TenantId);
            await _tenantLcsPayLogDAL.AddTenantLcsPayLog(new EtTenantLcsPayLog()
            {
                AgentId = myTenant.AgentId,
                TenantId = request.TenantId,
                AgtPayType = EmAgtPayType.Suixing,
                Attach = string.Empty,
                AuthNo = string.Empty,
                CreateOt = myActivityRouteItem.CreateTime,
                PayFinishOt = payTime,
                PayFinishDate = payTime.Date,
                DataType = EmTenantLcsPayLogDataType.Normal,
                IsDeleted = EmIsDeleted.Normal,
                MerchantName = myTenantSuixingAccount.MerName,
                MerchantNo = myTenantSuixingAccount.Mno,
                MerchantType = EmLcsMerchanttype.Ordinary,
                OpenId = myActivityRouteItem.OpenId,
                OrderBody = $"参加活动：{myActivity.Title}",
                OrderDesc = string.Empty,
                OrderNo = myActivityRouteItem.PayOrderNo,
                OutTradeNo = myActivityRouteItem.PayUuid,
                OrderSource = EmLcsPayLogOrderSource.MiniProgram,
                OrderType = EmLcsPayLogOrderType.Activity,
                OutRefundNo = string.Empty,
                PayType = EmLcsPayType.WeChat,
                RefundDate = null,
                RefundFee = string.Empty,
                RefundOt = null,
                RelationId = myActivityRouteItem.Id,
                Status = EmLcsPayLogStatus.PaySuccess,
                SubAppid = string.Empty,
                TerminalId = string.Empty,
                TerminalTrace = myActivityRouteItem.PayUuid,
                TotalFee = (myActivityRouteItem.PaySum * 100).ToString(),
                TotalFeeDesc = myActivityRouteItem.PaySum.EtmsToString2(),
                TotalFeeValue = myActivityRouteItem.PaySum,
                Remark = string.Empty,
                StudentName = myActivityRouteItem.StudentName,
                StudentPhone = myActivityRouteItem.StudentPhone,
            });
            _eventPublisher.Publish(new StatisticsLcsPayEvent(request.TenantId)
            {
                StatisticsDate = payTime.Date
            });
        }

        public async Task SuixingRefundCallbackConsumerEvent(SuixingRefundCallbackEvent request)
        {
            var myRouteItemId = request.ActivityRouteItemId;
            var myActivityRouteItem = await _activityRouteDAL.GetActivityRouteItemTemp(myRouteItemId);
            if (myActivityRouteItem.PayStatus == EmActivityRoutePayStatus.Refunded)
            {
                return;
            }
            await _activityRouteDAL.UpdateActivityRouteItemAboutRefundTemp(myRouteItemId, myActivityRouteItem.ActivityRouteId);
            if (myActivityRouteItem.IsTeamLeader)
            {
                await _activityRouteDAL.UpdateActivityRouteAboutRefundTemp(myActivityRouteItem.ActivityRouteId);
            }
            var myActivityRoute = await _activityRouteDAL.GetActivityRouteTemp(myActivityRouteItem.ActivityRouteId);
            var myActivity = await _activityMainDAL.GetActivityMain(myActivityRoute.ActivityId);
            _eventPublisher.Publish(new SyncActivityRouteFinishCountEvent(myActivityRouteItem.TenantId)
            {
                ActivityId = myActivityRouteItem.ActivityId,
                CountLimit = myActivityRoute.CountLimit,
                ActivityRouteId = myActivityRoute.Id,
                ActivityRouteItemId = myActivityRouteItem.Id,
                ActivityType = myActivityRouteItem.ActivityType,
                RuleContent = myActivity.RuleContent
            });
            await _tenantLcsPayLogDAL.UpdateTenantLcsPayLogRefund(EmAgtPayType.Suixing, myRouteItemId);
        }

        public async Task ActivityAutoRefundTenantConsumerEvent(ActivityAutoRefundTenantEvent request)
        {
            var config = await _tenantConfig2DAL.GetTenantConfig();
            var activityConfig = config.ActivityConfig;
            if (!activityConfig.IsAutoRefund)
            {
                return;
            }
            var pagingRequest = new GetRouteMustRefundPagingRequest()
            {
                PageCurrent = 1,
                PageSize = 100,
                TenantId = request.TenantId
            };
            var itemResult = await _activityRouteDAL.GetPagingRoute(pagingRequest);
            if (itemResult.Item2 == 0)
            {
                return;
            }
            ProcessAutoRefundTenantConsumerEvent(request.TenantId, itemResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(itemResult.Item2, pagingRequest.PageSize);
            pagingRequest.PageCurrent++;
            while (pagingRequest.PageCurrent <= totalPage)
            {
                itemResult = await _activityRouteDAL.GetPagingRoute(pagingRequest);
                ProcessAutoRefundTenantConsumerEvent(request.TenantId, itemResult.Item1);
                pagingRequest.PageCurrent++;
            }
        }

        private void ProcessAutoRefundTenantConsumerEvent(int tenantId, IEnumerable<EtActivityRoute> items)
        {
            foreach (var item in items)
            {
                _eventPublisher.Publish(new ActivityAutoRefundRouteEvent(tenantId)
                {
                    ActivityRouteId = item.Id
                });
            }
        }

        public async Task ActivityAutoRefundRouteConsumerEvent(ActivityAutoRefundRouteEvent request)
        {
            var pagingRequest = new GetRouteItemMustRefundPagingRequest()
            {
                PageCurrent = 1,
                PageSize = 100,
                TenantId = request.TenantId,
                ActivityRouteId = request.ActivityRouteId
            };
            var itemResult = await _activityRouteDAL.GetPagingRouteItem(pagingRequest);
            if (itemResult.Item2 == 0)
            {
                return;
            }
            ProcessAutoRefundRouteConsumerEvent(request.TenantId, itemResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(itemResult.Item2, pagingRequest.PageSize);
            pagingRequest.PageCurrent++;
            while (pagingRequest.PageCurrent <= totalPage)
            {
                itemResult = await _activityRouteDAL.GetPagingRouteItem(pagingRequest);
                ProcessAutoRefundRouteConsumerEvent(request.TenantId, itemResult.Item1);
                pagingRequest.PageCurrent++;
            }
        }

        private void ProcessAutoRefundRouteConsumerEvent(int tenantId, IEnumerable<EtActivityRouteItem> items)
        {
            foreach (var item in items)
            {
                _eventPublisher.Publish(new ActivityAutoRefundRouteItemEvent(tenantId)
                {
                    ActivityRouteItemId = item.Id
                });
            }
        }

        public async Task ActivityAutoRefundRouteItemConsumerEvent(ActivityAutoRefundRouteItemEvent request)
        {
            var myActivityRouteItem = await _activityRouteDAL.GetActivityRouteItem(request.ActivityRouteItemId);
            var myActivityRoute = await _activityRouteDAL.GetActivityRoute(myActivityRouteItem.ActivityRouteId);
            if (myActivityRoute.CountFinish >= myActivityRoute.CountLimit)
            {
                return;
            }
            if (myActivityRouteItem.PayStatus != EmActivityRoutePayStatus.Paid)
            {
                return;
            }
            var refundResult = _paySuixingService.Refund(new RefundReq()
            {
                ordNo = OrderNumberLib.SuixingRefundOrder(),
                mno = myActivityRouteItem.PayMno,
                origUuid = myActivityRouteItem.PayUuid,
                amt = myActivityRouteItem.PaySum,
                notifyUrl = SysWebApiAddressConfig.SuixingRefundCallbackUrl,
                refundReason = "活动失败",
                extend = $"{myActivityRouteItem.TenantId}_{myActivityRouteItem.Id}"
            });
            if (refundResult != null && refundResult.bizCode == EmBizCode.Success)
            {
                _eventPublisher.Publish(new SuixingRefundCallbackEvent(myActivityRouteItem.TenantId)
                {
                    ActivityRouteItemId = myActivityRouteItem.Id,
                    RefundTime = DateTime.Now
                });
            }
            else
            {
                LOG.Log.Warn($"[ActivityAutoRefundRouteItemConsumerEvent]退款失败", request, this.GetType());
            }
        }
    }
}
