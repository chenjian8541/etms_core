using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("TenantClassTimesTodayQueue")]
    public class TenantClassTimesTodayConsumer : ConsumerBase<TenantClassTimesTodayEvent>
    {
        protected override async Task Receive(TenantClassTimesTodayEvent eEvent)
        {
            var jobAnalyzeBLL = CustomServiceLocator.GetInstance<IJobAnalyzeBLL>();
            jobAnalyzeBLL.InitTenantId(eEvent.TenantId);
            await jobAnalyzeBLL.TenantClassTimesTodayConsumerEvent(eEvent);
        }
    }
}

