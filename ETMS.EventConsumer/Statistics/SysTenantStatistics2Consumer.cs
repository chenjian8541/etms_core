using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Statistics
{
    [QueueConsumerAttribution("SysTenantStatistics2MyQueue")]
    public class SysTenantStatistics2Consumer : ConsumerBase<SysTenantStatistics2Event>
    {
        protected override async Task Receive(SysTenantStatistics2Event eEvent)
        {
            var _lockKey = new SysTenantStatistics2Token(eEvent.TenantId);
            var _distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            var tenantLibBLL = CustomServiceLocator.GetInstance<ITenantLibBLL>();
            tenantLibBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SysTenantStatistics2Token, SysTenantStatistics2Event>(_lockKey, eEvent, this.ClassName,
                async () =>
                await tenantLibBLL.SysTenantStatistics2ConsumerEvent(eEvent)
                , false, true);
            await lockTakeHandler.Process();
        }
    }
}
