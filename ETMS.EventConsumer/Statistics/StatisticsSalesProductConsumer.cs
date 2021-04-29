using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsSalesProductQueue")]
    public class StatisticsSalesProductConsumer : ConsumerBase<StatisticsSalesProductEvent>
    {
        protected override async Task Receive(StatisticsSalesProductEvent eEvent)
        {
            var _lockKey = new StatisticsSalesProductToken(eEvent.TenantId, eEvent.StatisticsDate);
            var statisticsSalesBLL = CustomServiceLocator.GetInstance<IStatisticsSalesBLL>();
            statisticsSalesBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsSalesProductToken>(_lockKey, eEvent, this.ClassName,
                async () =>
                await statisticsSalesBLL.StatisticsSalesProductConsumeEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
