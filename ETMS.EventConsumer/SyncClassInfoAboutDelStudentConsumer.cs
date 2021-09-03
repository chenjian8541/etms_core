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
    [QueueConsumerAttribution("SyncClassInfoAboutDelStudentQueue")]
    public class SyncClassInfoAboutDelStudentConsumer : ConsumerBase<SyncClassInfoAboutDelStudentEvent>
    {
        protected override async Task Receive(SyncClassInfoAboutDelStudentEvent eEvent)
        {
            var _lockKey = new SyncClassInfoAboutDelStudentToken(eEvent.TenantId);
            var classBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            classBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncClassInfoAboutDelStudentToken, SyncClassInfoAboutDelStudentEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await classBLL.SyncClassInfoAboutDelStudentProcessEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
