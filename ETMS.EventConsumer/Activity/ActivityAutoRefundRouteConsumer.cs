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
    [QueueConsumerAttribution("ActivityAutoRefundRouteQueue")]
    public class ActivityAutoRefundRouteConsumer : ConsumerBase<ActivityAutoRefundRouteEvent>
    {
        protected override async Task Receive(ActivityAutoRefundRouteEvent eEvent)
        {
            var evActivityBLL = CustomServiceLocator.GetInstance<IEvActivityBLL>();
            evActivityBLL.InitTenantId(eEvent.TenantId);
            await evActivityBLL.ActivityAutoRefundRouteConsumerEvent(eEvent);
        }
    }
}
