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
    [QueueConsumerAttribution("AutoSyncTenantClassDetailQueue")]
    public class AutoSyncTenantClassDetailConsumer : ConsumerBase<AutoSyncTenantClassDetailEvent>
    {
        protected override async Task Receive(AutoSyncTenantClassDetailEvent eEvent)
        {
            var _lockKey = new AutoSyncTenantClassDetailToken(eEvent.TenantId, eEvent.ClassId);
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<AutoSyncTenantClassDetailToken, AutoSyncTenantClassDetailEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evClassBLL.AutoSyncTenantClassDetailConsumerEvent(eEvent)
                , false);
            await lockTakeHandler.Process();
        }
    }
}
