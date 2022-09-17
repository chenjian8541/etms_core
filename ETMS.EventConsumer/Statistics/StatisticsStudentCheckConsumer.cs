using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Statistics
{
    [QueueConsumerAttribution("StatisticsStudentCheckQueue")]
    public class StatisticsStudentCheckConsumer : ConsumerBase<StatisticsStudentCheckEvent>
    {
        protected override async Task Receive(StatisticsStudentCheckEvent eEvent)
        {
            var _lockKey = new StatisticsStudentCheckToken(eEvent.TenantId);
            var evStudentCheckOnBLL = CustomServiceLocator.GetInstance<IEvStudentCheckOnBLL>();
            evStudentCheckOnBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsStudentCheckToken, StatisticsStudentCheckEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evStudentCheckOnBLL.StatisticsStudentCheckConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
