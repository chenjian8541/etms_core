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
    [QueueConsumerAttribution("TenantAgentStatisticsQueue")]
    public class TenantAgentStatisticsConsumer : ConsumerBase<TenantAgentStatisticsEvent>
    {
        protected override async Task Receive(TenantAgentStatisticsEvent @event)
        {
            var _lockKey = new TenantAgentStatisticsToken(@event.TenantId);
            var tenantDataCollectBLL = CustomServiceLocator.GetInstance<ITenantDataCollect>();
            tenantDataCollectBLL.InitTenantId(@event.TenantId);
            var lockTakeHandler = new LockTakeHandler<TenantAgentStatisticsToken, TenantAgentStatisticsEvent>(_lockKey, @event, this.ClassName,
                async () =>
                await tenantDataCollectBLL.TenantAgentStatisticsConsumerEvent(@event)
                );
            await lockTakeHandler.Process();
        }
    }
}
