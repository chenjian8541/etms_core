using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("CloudFileAutoDelDayQueue")]
    public class CloudFileAutoDelDayConsumer : ConsumerBase<CloudFileAutoDelDayEvent>
    {
        protected override async Task Receive(CloudFileAutoDelDayEvent eEvent)
        {
            var cloudFileAutoDelDayBLL = CustomServiceLocator.GetInstance<ICloudFileAutoDelDayBLL>();
            cloudFileAutoDelDayBLL.InitTenantId(eEvent.TenantId);
            await cloudFileAutoDelDayBLL.CloudFileAutoDelDayConsumerEvent(eEvent);
        }
    }
}
