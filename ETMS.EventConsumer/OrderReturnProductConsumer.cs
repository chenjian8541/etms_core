using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("OrderReturnProductQueue")]
    public class OrderReturnProductConsumer : ConsumerBase<OrderReturnProductEvent>
    {
        protected override async Task Receive(OrderReturnProductEvent eEvent)
        {
            var orderRepealProcessBLL = CustomServiceLocator.GetInstance<IOrderHandleProcessBLL>();
            orderRepealProcessBLL.InitTenantId(eEvent.TenantId);
            await orderRepealProcessBLL.OrderReturnProductEventProcess(eEvent);
        }
    }
}