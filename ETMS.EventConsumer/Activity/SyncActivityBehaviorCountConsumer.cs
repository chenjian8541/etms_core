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
    [QueueConsumerAttribution("SyncActivityBehaviorCountQueue")]
    public class SyncActivityBehaviorCountConsumer : ConsumerBase<SyncActivityBehaviorCountEvent>
    {
        protected override async Task Receive(SyncActivityBehaviorCountEvent eEvent)
        {
            var _lockKey = new SyncActivityBehaviorCountToken(eEvent.TenantId, eEvent.ActivityId);
            var evActivityBLL = CustomServiceLocator.GetInstance<IEvActivityBLL>();
            evActivityBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncActivityBehaviorCountToken, SyncActivityBehaviorCountEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evActivityBLL.SyncActivityBehaviorCountConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
