using ETMS.Business.Common;
using ETMS.Business.WxCore;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.View.Activity;
using ETMS.Event.DataContract.Activity;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess.Activity;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
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
        public EvActivityBLL(IAppConfigurtaionServices appConfigurtaionServices, IActivityMainDAL activityMainDAL, IActivityRouteDAL activityRouteDAL,
           IActivityVisitorDAL activityVisitorDAL, ISysActivityRouteItemDAL sysActivityRouteItemDAL, IEventPublisher eventPublisher)
            : base(appConfigurtaionServices)
        {
            this._activityMainDAL = activityMainDAL;
            this._activityRouteDAL = activityRouteDAL;
            this._activityVisitorDAL = activityVisitorDAL;
            this._sysActivityRouteItemDAL = sysActivityRouteItemDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _activityMainDAL, _activityRouteDAL, _activityVisitorDAL);
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
            await _activityMainDAL.SetEffectCount(request.ActivityId, joinCount, routeCount, finishCount);
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
            if (status != EmSysActivityRouteItemStatus.Going)
            {
                await _activityRouteDAL.SetActivityRouteItemStatus(request.ActivityRouteId, status);
                await _sysActivityRouteItemDAL.UpdateActivityRouteItemStatus(request.TenantId, request.ActivityId, request.ActivityRouteId, status);
            }
            _eventPublisher.Publish(new SyncSysActivityRouteItemEvent(request.TenantId)
            {
                ActivityRouteItemId = request.ActivityRouteItemId,
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
            var scene = $"{request.TenantId}_{id}";
            var routeShareQRCodeKey = await GenerateQrCode(request.TenantId,
                AliyunOssFileTypeEnum.ActivityRouteQrCode, key, MiniProgramPathConfig.ActivityRoute, scene);
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
                    Status = item.Status
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
                await _sysActivityRouteItemDAL.EdiSysActivityRouteItem(log);
            }
        }
    }
}
