using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncClassTimesStudentQueue")]
    public class SyncClassTimesStudentConsumer : ConsumerBase<SyncClassTimesStudentEvent>
    {
        protected override async Task Receive(SyncClassTimesStudentEvent eEvent)
        {
            var _lockKey = new SyncClassTimesStudentConsumerToken(eEvent.TenantId, eEvent.ClassTimesId);
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncClassTimesStudentConsumerToken>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evClassBLL.SyncClassTimesStudentConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
