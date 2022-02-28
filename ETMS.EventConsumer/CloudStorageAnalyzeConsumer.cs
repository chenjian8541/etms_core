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
    [QueueConsumerAttribution("CloudStorageAnalyzeQueue")]
    public class CloudStorageAnalyzeConsumer : ConsumerBase<CloudStorageAnalyzeEvent>
    {
        protected override async Task Receive(CloudStorageAnalyzeEvent @event)
        {
            var tenantLibBLL = CustomServiceLocator.GetInstance<ITenantLibBLL>();
            tenantLibBLL.InitTenantId(@event.TenantId);
            await tenantLibBLL.CloudStorageAnalyzeConsumerEvent(@event);
        }
    }
}
