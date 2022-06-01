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
    [QueueConsumerAttribution("AutoCheckSignTenantQueue")]
    public class AutoCheckSignTenantConsumer : ConsumerBase<AutoCheckSignTenantEvent>
    {
        protected override async Task Receive(AutoCheckSignTenantEvent eEvent)
        {
            var _lockKey = new AutoCheckSignTenantToken(eEvent.TenantId);
            var evAutoCheckSignBLL = CustomServiceLocator.GetInstance<IEvAutoCheckSignBLL>();
            evAutoCheckSignBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<AutoCheckSignTenantToken, AutoCheckSignTenantEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evAutoCheckSignBLL.AutoCheckSignTenantConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
