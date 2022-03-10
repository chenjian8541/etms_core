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
    [QueueConsumerAttribution("StudentCourseRestoreTimeBatchQueue")]
    public class StudentCourseRestoreTimeBatchConsumer : ConsumerBase<StudentCourseRestoreTimeBatchEvent>
    {
        protected override async Task Receive(StudentCourseRestoreTimeBatchEvent eEvent)
        {
            var _lockKey = new StudentCourseRestoreTimeBatchToken(eEvent.TenantId, eEvent.StudentId);
            var evStudentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            evStudentBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StudentCourseRestoreTimeBatchToken, StudentCourseRestoreTimeBatchEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evStudentBLL.StudentCourseRestoreTimeBatchConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
