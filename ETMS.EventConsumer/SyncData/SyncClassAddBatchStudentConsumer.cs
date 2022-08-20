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


namespace ETMS.EventConsumer.SyncData
{
    [QueueConsumerAttribution("SyncClassAddBatchStudentQueue")]
    public class SyncClassAddBatchStudentConsumer : ConsumerBase<SyncClassAddBatchStudentEvent>
    {
        protected override async Task Receive(SyncClassAddBatchStudentEvent eEvent)
        {
            var _lockKey = new SyncClassAddBatchStudentToken(eEvent.TenantId, eEvent.ClassId);
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncClassAddBatchStudentToken, SyncClassAddBatchStudentEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evClassBLL.SyncClassAddBatchStudentConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
