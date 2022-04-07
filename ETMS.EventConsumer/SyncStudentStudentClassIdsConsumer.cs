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
    [QueueConsumerAttribution("SyncStudentStudentClassIdsQueue")]
    public class SyncStudentStudentClassIdsConsumer : ConsumerBase<SyncStudentStudentClassIdsEvent>
    {
        protected override async Task Receive(SyncStudentStudentClassIdsEvent @event)
        {
            var _lockKey = new SyncStudentStudentClassIdsToken(@event.TenantId, @event.StudentId);
            var studentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            studentBLL.InitTenantId(@event.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncStudentStudentClassIdsToken, SyncStudentStudentClassIdsEvent>(_lockKey, @event, this.ClassName,
                async () =>
                await studentBLL.SyncStudentStudentClassIdsConsumerEvent(@event)
                );
            await lockTakeHandler.Process();
        }
    }
}
