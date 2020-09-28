using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("GiftExchangeQueue")]
    public class GiftExchangeConsumer : ConsumerBase<GiftExchangeEvent>
    {
        protected override async Task Receive(GiftExchangeEvent giftExchange)
        {
            var giftBLL = CustomServiceLocator.GetInstance<IGiftBLL>();
            giftBLL.InitTenantId(giftExchange.TenantId);
            await giftBLL.GiftExchangeEvent(giftExchange);
        }
    }
}
