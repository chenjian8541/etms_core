using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("OrderStudentEnrolmentRepealQueue")]
    public class OrderStudentEnrolmentRepealConsumer : ConsumerBase<OrderStudentEnrolmentRepealEvent>
    {
        protected override async Task Receive(OrderStudentEnrolmentRepealEvent eEvent)
        {
            var orderRepealProcessBLL = CustomServiceLocator.GetInstance<IOrderRepealProcessBLL>();
            orderRepealProcessBLL.InitTenantId(eEvent.TenantId);
            await orderRepealProcessBLL.OrderStudentEnrolmentRepealEventProcess(eEvent);
        }
    }
}
