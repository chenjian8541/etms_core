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

namespace ETMS.EventConsumer.TeacherSalary
{
    [QueueConsumerAttribution("StatisticsTeacherSalaryClassDayQueue")]
    public class StatisticsTeacherSalaryClassDayConsumer : ConsumerBase<StatisticsTeacherSalaryClassDayEvent>
    {
        protected override async Task Receive(StatisticsTeacherSalaryClassDayEvent eEvent)
        {
            var _lockKey = new StatisticsTeacherSalaryClassDayToken(eEvent.TenantId, eEvent.Time);
            var evEducationBLL = CustomServiceLocator.GetInstance<IEvEducationBLL>();
            evEducationBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsTeacherSalaryClassDayToken, StatisticsTeacherSalaryClassDayEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evEducationBLL.StatisticsTeacherSalaryClassDayConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
