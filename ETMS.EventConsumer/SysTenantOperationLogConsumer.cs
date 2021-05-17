using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SysTenantOperationLogQueue")]
    public class SysTenantOperationLogConsumer : ConsumerBase<SysTenantOperationLogEvent>
    {
        protected override async Task Receive(SysTenantOperationLogEvent @event)
        {
            var tenantDataCollectBLL = CustomServiceLocator.GetInstance<ITenantDataCollect>();
            tenantDataCollectBLL.InitTenantId(@event.TenantId);
            await tenantDataCollectBLL.SysTenantOperationLogConsumerEvent(@event);
        }
    }
}