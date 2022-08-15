using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("AutoSyncTenantClassQueue")]
    public class AutoSyncTenantClassConsumer: ConsumerBase<AutoSyncTenantClassEvent>
    {
        protected override async Task Receive(AutoSyncTenantClassEvent eEvent)
        {
            var _lockKey = new AutoSyncTenantClassToken(eEvent.TenantId);
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<AutoSyncTenantClassToken, AutoSyncTenantClassEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evClassBLL.AutoSyncTenantClassConsumerEvent(eEvent)
                , false);
            await lockTakeHandler.Process();
        }
    }
}
