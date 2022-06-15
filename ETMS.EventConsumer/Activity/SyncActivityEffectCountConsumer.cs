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
    [QueueConsumerAttribution("SyncActivityEffectCountQueue")]
    public class SyncActivityEffectCountConsumer : ConsumerBase<SyncActivityEffectCountEvent>
    {
        protected override async Task Receive(SyncActivityEffectCountEvent eEvent)
        {
            var _lockKey = new SyncActivityEffectCountToken(eEvent.TenantId, eEvent.ActivityId);
            var evActivityBLL = CustomServiceLocator.GetInstance<IEvActivityBLL>();
            evActivityBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncActivityEffectCountToken, SyncActivityEffectCountEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evActivityBLL.SyncActivityEffectCountConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
