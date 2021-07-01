using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncClassInfoQueue")]
    public class SyncClassInfoConsumer : ConsumerBase<SyncClassInfoEvent>
    {
        protected override async Task Receive(SyncClassInfoEvent eEvent)
        {
            var _lockKey = new SyncClassInfoConsumerToken(eEvent.TenantId, eEvent.ClassId);
            var classBLL = CustomServiceLocator.GetInstance<IClassBLL>();
            classBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncClassInfoConsumerToken, SyncClassInfoEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await classBLL.SyncClassInfoProcessEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
