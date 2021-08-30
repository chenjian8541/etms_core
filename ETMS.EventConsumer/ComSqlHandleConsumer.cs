using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ComSqlHandleQueue")]
    public class ComSqlHandleConsumer : ConsumerBase<ComSqlHandleEvent>
    {
        protected override async Task Receive(ComSqlHandleEvent eEvent)
        {
            var _lockKey = new ComSqlHandleToken(eEvent.TenantId);
            var tenantLibBLL = CustomServiceLocator.GetInstance<ITenantLibBLL>();
            tenantLibBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<ComSqlHandleToken, ComSqlHandleEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await tenantLibBLL.ComSqlHandleConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
