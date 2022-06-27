using ETMS.Business.WxCore;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Event.DataContract.Activity;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess.Activity;
using ETMS.IDataAccess.EtmsManage;
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

        public EvActivityBLL(IAppConfigurtaionServices appConfigurtaionServices, IActivityMainDAL activityMainDAL, IActivityRouteDAL activityRouteDAL,
           IActivityVisitorDAL activityVisitorDAL, ISysActivityRouteItemDAL sysActivityRouteItemDAL)
            : base(appConfigurtaionServices)
        {
            this._activityMainDAL = activityMainDAL;
            this._activityRouteDAL = activityRouteDAL;
            this._activityVisitorDAL = activityVisitorDAL;
            this._sysActivityRouteItemDAL = sysActivityRouteItemDAL;
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

        public async Task SyncActivityRouteFinishCountConsumerEvent(SyncActivityRouteFinishCountEvent request)
        {
            var countFinish = await _activityRouteDAL.GetActivityRouteFinishCount(request.ActivityRouteId, request.ActivityType);
            await _activityRouteDAL.SetFinishCount(request.ActivityRouteId, countFinish);
            if (countFinish >= request.CountLimit)
            {
                await _sysActivityRouteItemDAL.UpdateActivityRouteItemStatus(
                    request.TenantId, request.ActivityId, request.ActivityRouteId, EmSysActivityRouteItemStatus.Finish);
            }
        }

        public async Task SyncActivityBascInfoConsumerEvent(SyncActivityBascInfoEvent request)
        {
            await _activityRouteDAL.SyncActivityBascInfo(request.NewActivityMain);
            await _sysActivityRouteItemDAL.SyncActivityBascInfo(request.NewActivityMain);
        }

        public async Task CalculateActivityRouteIInfoEvent(CalculateActivityRouteIInfoEvent request)
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
    }
}
