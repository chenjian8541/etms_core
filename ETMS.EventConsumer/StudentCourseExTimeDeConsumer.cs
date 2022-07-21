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
    [QueueConsumerAttribution("StudentCourseExTimeDeQueue")]
    internal class StudentCourseExTimeDeConsumer : ConsumerBase<StudentCourseExTimeDeEvent>
    {
        protected override async Task Receive(StudentCourseExTimeDeEvent eEvent)
        {
            var _lockKey = new StudentCourseExTimeDeToken(eEvent.TenantId, eEvent.StudentId, eEvent.CourseId);
            var evEducationBLL = CustomServiceLocator.GetInstance<IEvEducationBLL>();
            evEducationBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StudentCourseExTimeDeToken, StudentCourseExTimeDeEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evEducationBLL.StudentCourseExTimeDeConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
