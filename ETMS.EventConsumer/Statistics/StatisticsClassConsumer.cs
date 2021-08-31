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
    [QueueConsumerAttribution("StatisticsClassQueue")]
    public class StatisticsClassConsumer : ConsumerBase<StatisticsClassEvent>
    {
        protected override async Task Receive(StatisticsClassEvent eEvent)
        {
            var _lockKey = new StatisticsClassToken(eEvent.TenantId, eEvent.ClassOt);
            var statisticsClassBLL = CustomServiceLocator.GetInstance<IStatisticsClassBLL>();
            statisticsClassBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsClassToken, StatisticsClassEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await statisticsClassBLL.StatisticsClassConsumeEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
