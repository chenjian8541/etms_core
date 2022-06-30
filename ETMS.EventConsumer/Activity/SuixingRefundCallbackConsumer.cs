using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Activity;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Activity
{
    [QueueConsumerAttribution("SuixingRefundCallbackQueue")]
    public class SuixingRefundCallbackConsumer: ConsumerBase<SuixingRefundCallbackEvent>
    {
        protected override async Task Receive(SuixingRefundCallbackEvent eEvent)
        {
            var _lockKey = new SuixingRefundCallbackToken(eEvent.TenantId, eEvent.ActivityRouteItemId);
            var evActivityBLL = CustomServiceLocator.GetInstance<IEvActivityBLL>();
            evActivityBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SuixingRefundCallbackToken, SuixingRefundCallbackEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evActivityBLL.SuixingRefundCallbackConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
