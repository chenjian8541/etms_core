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
    [QueueConsumerAttribution("SyncStudentClassInfoQueue")]
    public class SyncStudentClassInfoConsumer : ConsumerBase<SyncStudentClassInfoEvent>
    {
        protected override async Task Receive(SyncStudentClassInfoEvent eEvent)
        {
            var _lockKey = new SyncStudentClassInfoToken(eEvent.TenantId, eEvent.StudentId);
            var studentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            studentBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncStudentClassInfoToken, SyncStudentClassInfoEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await studentBLL.SyncStudentClassInfoConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
