using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("UpdateTenantIpAddressQueue")]
    internal class UpdateTenantIpAddressConsumer : ConsumerBase<UpdateTenantIpAddressEvent>
    {
        protected override async Task Receive(UpdateTenantIpAddressEvent @event)
        {
            var _lockKey = new UpdateTenantIpAddressToken(@event.TenantId);
            var tenantLibBLL = CustomServiceLocator.GetInstance<ITenantLibBLL>();
            tenantLibBLL.InitTenantId(@event.TenantId);
            var lockTakeHandler = new LockTakeHandler<UpdateTenantIpAddressToken, UpdateTenantIpAddressEvent>(_lockKey, @event, this.ClassName,
                async () =>
                await tenantLibBLL.UpdateTenantIpAddressConsumerEvent(@event)
                );
            await lockTakeHandler.Process();
        }
    }
}
