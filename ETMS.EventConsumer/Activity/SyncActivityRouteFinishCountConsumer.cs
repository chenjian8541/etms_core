using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Activity;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Activity
{
    [QueueConsumerAttribution("SyncActivityRouteFinishCountQueue")]
    public class SyncActivityRouteFinishCountConsumer : ConsumerBase<SyncActivityRouteFinishCountEvent>
    {
        protected override async Task Receive(SyncActivityRouteFinishCountEvent eEvent)
        {
            var _lockKey = new SyncActivityRouteFinishCountToken(eEvent.TenantId, eEvent.ActivityRouteId);
            var evActivityBLL = CustomServiceLocator.GetInstance<IEvActivityBLL>();
            evActivityBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncActivityRouteFinishCountToken, SyncActivityRouteFinishCountEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evActivityBLL.SyncActivityRouteFinishCountConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
