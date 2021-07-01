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
    [QueueConsumerAttribution("StatisticsStudentQueue")]
    public class StatisticsStudentConsumer : ConsumerBase<StatisticsStudentEvent>
    {
        protected override async Task Receive(StatisticsStudentEvent eEvent)
        {
            var _lockKey = new StatisticsStudentToken(eEvent.TenantId, eEvent.StatisticsDate, eEvent.OpType);
            var statisticsStudentBLL = CustomServiceLocator.GetInstance<IStatisticsStudentBLL>();
            statisticsStudentBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsStudentToken, StatisticsStudentEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await statisticsStudentBLL.StatisticsStudentConsumeEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
