using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract.Activity;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvActivityBLL : IEvActivityBLL
    {
        private readonly IActivityMainDAL _activityMainDAL;

        private readonly IActivityRouteDAL _activityRouteDAL;

        private readonly IActivityVisitorDAL _activityVisitorDAL;

        public EvActivityBLL(IActivityMainDAL activityMainDAL, IActivityRouteDAL activityRouteDAL,
           IActivityVisitorDAL activityVisitorDAL)
        {
            this._activityMainDAL = activityMainDAL;
            this._activityRouteDAL = activityRouteDAL;
            this._activityVisitorDAL = activityVisitorDAL;
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
                        OpenId = request.OpenId,
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
        }

        public async Task SyncActivityBascInfoConsumerEvent(SyncActivityBascInfoEvent request)
        {
            await _activityRouteDAL.SyncActivityBascInfo(request.NewActivityMain);
        }
    }
}
