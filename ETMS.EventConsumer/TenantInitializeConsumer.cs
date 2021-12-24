using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("TenantInitializeQueue")]
    public class TenantInitializeConsumer : ConsumerBase<TenantInitializeEvent>
    {
        protected override async Task Receive(TenantInitializeEvent @event)
        {
            var tenantInitializeBLL = CustomServiceLocator.GetInstance<ITenantInitializeBLL>();
            tenantInitializeBLL.InitTenantId(@event.TenantId);
            await tenantInitializeBLL.TenantInitializeConsumerEvent(@event);
        }
    }
}
