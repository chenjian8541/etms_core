using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Statistics
{
    [QueueConsumerAttribution("SysTenantStatisticsWeekAndMonthQueue")]
    public class SysTenantStatisticsWeekAndMonthConsumer : ConsumerBase<SysTenantStatisticsWeekAndMonthEvent>
    {
        protected override async Task Receive(SysTenantStatisticsWeekAndMonthEvent eEvent)
        {
            var _lockKey = new SysTenantStatisticsWeekAndMonthToken(eEvent.TenantId);
            var tenantLibBLL = CustomServiceLocator.GetInstance<ITenantLibBLL>();
            tenantLibBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SysTenantStatisticsWeekAndMonthToken, SysTenantStatisticsWeekAndMonthEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await tenantLibBLL.SysTenantStatisticsWeekAndMonthConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
