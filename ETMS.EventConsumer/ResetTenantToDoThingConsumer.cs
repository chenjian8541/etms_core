using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ResetTenantToDoThingQueue")]
    public class ResetTenantToDoThingConsumer : ConsumerBase<ResetTenantToDoThingEvent>
    {
        protected override async Task Receive(ResetTenantToDoThingEvent giftExchange)
        {
            var statisticsTenantBLL = CustomServiceLocator.GetInstance<IStatisticsTenantBLL>();
            statisticsTenantBLL.InitTenantId(giftExchange.TenantId);
            await statisticsTenantBLL.ResetTenantToDoThingConsumerEvent(giftExchange);
        }
    }
}
