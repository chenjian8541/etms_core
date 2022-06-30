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
    [QueueConsumerAttribution("SuixingPayCallbackQueue")]
    public class SuixingPayCallbackConsumer : ConsumerBase<SuixingPayCallbackEvent>
    {
        protected override async Task Receive(SuixingPayCallbackEvent eEvent)
        {
            var _lockKey = new SuixingPayCallbackToken(eEvent.TenantId, eEvent.ActivityRouteItemId);
            var evActivityBLL = CustomServiceLocator.GetInstance<IEvActivityBLL>();
            evActivityBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SuixingPayCallbackToken, SuixingPayCallbackEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evActivityBLL.SuixingPayCallbackConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
