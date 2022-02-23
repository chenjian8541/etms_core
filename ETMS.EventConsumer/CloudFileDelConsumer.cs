using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("CloudFileDelQueue")]
    public class CloudFileDelConsumer : ConsumerBase<CloudFileDelEvent>
    {
        protected override async Task Receive(CloudFileDelEvent eEvent)
        {
            var cloudFileCleanBLL = CustomServiceLocator.GetInstance<IEvCloudFileCleanBLL>();
            cloudFileCleanBLL.InitTenantId(eEvent.TenantId);
            await cloudFileCleanBLL.CloudFileDelConsumerEvent(eEvent);
        }
    }
}
