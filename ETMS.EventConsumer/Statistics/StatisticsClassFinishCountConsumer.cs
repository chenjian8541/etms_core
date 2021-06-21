using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Statistics
{
    [QueueConsumerAttribution("StatisticsClassFinishCountQueue")]
    public class StatisticsClassFinishCountConsumer : ConsumerBase<StatisticsClassFinishCountEvent>
    {
        protected override async Task Receive(StatisticsClassFinishCountEvent eEvent)
        {
            var _lockKey = new StatisticsClassFinishCountToken(eEvent.TenantId);
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsClassFinishCountToken>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evClassBLL.StatisticsClassFinishCountConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
