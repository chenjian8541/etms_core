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
    [QueueConsumerAttribution("AutoCheckSignClassTimesQueue")]
    public class AutoCheckSignClassTimesConsumer : ConsumerBase<AutoCheckSignClassTimesEvent>
    {
        protected override async Task Receive(AutoCheckSignClassTimesEvent eEvent)
        {
            var _lockKey = new AutoCheckSignClassTimesToken(eEvent.TenantId, eEvent.ClassTimesId);
            var evAutoCheckSignBLL = CustomServiceLocator.GetInstance<IEvAutoCheckSignBLL>();
            evAutoCheckSignBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<AutoCheckSignClassTimesToken, AutoCheckSignClassTimesEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evAutoCheckSignBLL.AutoCheckSignClassTimesConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
