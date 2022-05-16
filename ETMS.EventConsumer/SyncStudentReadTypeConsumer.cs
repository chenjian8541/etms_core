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
    [QueueConsumerAttribution("SyncStudentReadTypeQueue")]
    public class SyncStudentReadTypeConsumer : ConsumerBase<SyncStudentReadTypeEvent>
    {
        protected override async Task Receive(SyncStudentReadTypeEvent @event)
        {
            var _lockKey = new SyncStudentReadTypeToken(@event.TenantId, @event.StudentId);
            var studentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            studentBLL.InitTenantId(@event.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncStudentReadTypeToken, SyncStudentReadTypeEvent>(_lockKey, @event, this.ClassName,
                async () =>
                await studentBLL.SyncStudentReadTypeConsumerEvent(@event)
                );
            await lockTakeHandler.Process();
        }
    }
}
