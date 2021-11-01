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
    [QueueConsumerAttribution("SyncTenantLastOpTimeQueue")]
    public class SyncTenantLastOpTimeConsumer : ConsumerBase<SyncTenantLastOpTimeEvent>
    {

        protected override async Task Receive(SyncTenantLastOpTimeEvent request)
        {
            var tenantLibBLL = CustomServiceLocator.GetInstance<ITenantLibBLL>();
            tenantLibBLL.InitTenantId(request.TenantId);
            await tenantLibBLL.SyncTenantLastOpTimeConsumerEvent(request);
        }
    }
}
