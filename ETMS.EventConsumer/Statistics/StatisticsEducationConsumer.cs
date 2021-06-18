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
    [QueueConsumerAttribution("StatisticsEducationQueue")]
    public class StatisticsEducationConsumer : ConsumerBase<StatisticsEducationEvent>
    {
        protected override async Task Receive(StatisticsEducationEvent eEvent)
        {
            var _lockKey = new StatisticsEducationToken(eEvent.TenantId);
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsEducationToken>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evClassBLL.StatisticsEducationConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
