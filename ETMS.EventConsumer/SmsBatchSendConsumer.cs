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
    [QueueConsumerAttribution("SmsBatchSendQueue")]
    public class SmsBatchSendConsumer : ConsumerBase<SmsBatchSendEvent>
    {
        protected override async Task Receive(SmsBatchSendEvent eEvent)
        {
            var tenantLibBLL = CustomServiceLocator.GetInstance<ITenantLibBLL>();
            tenantLibBLL.InitTenantId(eEvent.TenantId);
            await tenantLibBLL.SmsBatchSendConsumerEvent(eEvent);
        }
    }
}
