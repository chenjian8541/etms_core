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
    [QueueConsumerAttribution("StatisticsStudentCountQueue")]
    public class StatisticsStudentCountConsumer : ConsumerBase<StatisticsStudentCountEvent>
    {
        protected override async Task Receive(StatisticsStudentCountEvent eEvent)
        {
            var _lockKey = new StatisticsStudentCountToken(eEvent.TenantId, eEvent.Time);
            var statisticsStudentBLL = CustomServiceLocator.GetInstance<IStatisticsStudentBLL>();
            statisticsStudentBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsStudentCountToken, StatisticsStudentCountEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await statisticsStudentBLL.StatisticsStudentCountConsumeEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
