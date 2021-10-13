using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Statistics
{
    [QueueConsumerAttribution("StatisticsLcsPayQueue")]
    public class StatisticsLcsPayConsumer : ConsumerBase<StatisticsLcsPayEvent>
    {
        protected override async Task Receive(StatisticsLcsPayEvent eEvent)
        {
            var _lockKey = new StatisticsLcsPayToken(eEvent.TenantId, eEvent.StatisticsDate);
            var statisticsTenantBLL = CustomServiceLocator.GetInstance<IStatisticsTenantBLL>();
            statisticsTenantBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsLcsPayToken, StatisticsLcsPayEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await statisticsTenantBLL.StatisticsLcsPayConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
