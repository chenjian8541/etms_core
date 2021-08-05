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
    [QueueConsumerAttribution("StatisticsTeacherSalaryClassTimesQueue")]
    public class StatisticsTeacherSalaryClassTimesConsumer : ConsumerBase<StatisticsTeacherSalaryClassTimesEvent>
    {
        protected override async Task Receive(StatisticsTeacherSalaryClassTimesEvent eEvent)
        {
            var _lockKey = new StatisticsTeacherSalaryClassTimesToken(eEvent.TenantId, eEvent.ClassRecordId);
            var evEducationBLL = CustomServiceLocator.GetInstance<IEvEducationBLL>();
            evEducationBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsTeacherSalaryClassTimesToken, StatisticsTeacherSalaryClassTimesEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evEducationBLL.StatisticsTeacherSalaryClassTimesConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
