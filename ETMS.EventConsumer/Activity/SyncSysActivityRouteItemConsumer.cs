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
    [QueueConsumerAttribution("SyncSysActivityRouteItemQueue")]
    public class SyncSysActivityRouteItemConsumer : ConsumerBase<SyncSysActivityRouteItemEvent>
    {
        protected override async Task Receive(SyncSysActivityRouteItemEvent eEvent)
        {
            var _lockKey = new SyncSysActivityRouteItemToken(eEvent.TenantId, eEvent.ActivityRouteItemId);
            var evActivityBLL = CustomServiceLocator.GetInstance<IEvActivityBLL>();
            evActivityBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncSysActivityRouteItemToken, SyncSysActivityRouteItemEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evActivityBLL.SyncSysActivityRouteItemConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
