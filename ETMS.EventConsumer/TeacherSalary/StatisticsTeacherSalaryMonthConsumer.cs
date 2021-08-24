using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsTeacherSalaryClassTimesQueue")]
    public class StatisticsTeacherSalaryMonthConsumer : ConsumerBase<StatisticsTeacherSalaryMonthEvent>
    {
        protected override async Task Receive(StatisticsTeacherSalaryMonthEvent eEvent)
        {
            var _lockKey = new StatisticsTeacherSalaryMonthToken(eEvent.TenantId);
            var evEducationBLL = CustomServiceLocator.GetInstance<IEvEducationBLL>();
            evEducationBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsTeacherSalaryMonthToken, StatisticsTeacherSalaryMonthEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evEducationBLL.StatisticsTeacherSalaryMonthConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
