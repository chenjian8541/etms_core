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
    [QueueConsumerAttribution("SyncStudentStudentCourseIdsQueue")]
    public class SyncStudentStudentCourseIdsConsumer : ConsumerBase<SyncStudentStudentCourseIdsEvent>
    {
        protected override async Task Receive(SyncStudentStudentCourseIdsEvent @event)
        {
            var _lockKey = new SyncStudentStudentCourseIdsToken(@event.TenantId, @event.StudentId);
            var studentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            studentBLL.InitTenantId(@event.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncStudentStudentCourseIdsToken, SyncStudentStudentCourseIdsEvent>(_lockKey, @event, this.ClassName,
                async () =>
                await studentBLL.SyncStudentStudentCourseIdsConsumerEvent(@event)
                );
            await lockTakeHandler.Process();
        }
    }
}
