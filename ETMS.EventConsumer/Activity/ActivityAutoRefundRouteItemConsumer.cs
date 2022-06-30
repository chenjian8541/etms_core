using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Activity;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Activity
{
    [QueueConsumerAttribution("ActivityAutoRefundRouteItemQueue")]
    public class ActivityAutoRefundRouteItemConsumer : ConsumerBase<ActivityAutoRefundRouteItemEvent>
    {
        protected override async Task Receive(ActivityAutoRefundRouteItemEvent eEvent)
        {
            var _lockKey = new ActivityAutoRefundRouteItemToken(eEvent.TenantId, eEvent.ActivityRouteItemId);
            var evActivityBLL = CustomServiceLocator.GetInstance<IEvActivityBLL>();
            evActivityBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<ActivityAutoRefundRouteItemToken, ActivityAutoRefundRouteItemEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evActivityBLL.ActivityAutoRefundRouteItemConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
