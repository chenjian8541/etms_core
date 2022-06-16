using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Activity;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Activity
{
    [QueueConsumerAttribution("SyncActivityBascInfoQueue")]
    public class SyncActivityBascInfoConsumer : ConsumerBase<SyncActivityBascInfoEvent>
    {
        protected override async Task Receive(SyncActivityBascInfoEvent eEvent)
        {
            var _lockKey = new SyncActivityBascInfoToken(eEvent.TenantId, eEvent.NewActivityMain.Id);
            var evActivityBLL = CustomServiceLocator.GetInstance<IEvActivityBLL>();
            evActivityBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncActivityBascInfoToken, SyncActivityBascInfoEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evActivityBLL.SyncActivityBascInfoConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
