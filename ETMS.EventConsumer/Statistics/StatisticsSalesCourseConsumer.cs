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
    [QueueConsumerAttribution("StatisticsSalesCourseQueue")]
    public class StatisticsSalesCourseConsumer : ConsumerBase<StatisticsSalesCourseEvent>
    {
        protected override async Task Receive(StatisticsSalesCourseEvent eEvent)
        {
            var _lockKey = new StatisticsSalesCourseToken(eEvent.TenantId, eEvent.StatisticsDate);
            var statisticsSalesCourseBLL = CustomServiceLocator.GetInstance<IStatisticsSalesCourseBLL>();
            statisticsSalesCourseBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsSalesCourseToken, StatisticsSalesCourseEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await statisticsSalesCourseBLL.StatisticsSalesCourseConsumeEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
