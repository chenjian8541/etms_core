using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ActiveWxMessageAddQueue")]
    public class ActiveWxMessageAddConsumer : ConsumerBase<ActiveWxMessageAddEvent>
    {
        protected override async Task Receive(ActiveWxMessageAddEvent eEvent)
        {
            var activeWxMessageBLL = CustomServiceLocator.GetInstance<IActiveWxMessageBLL>();
            activeWxMessageBLL.InitTenantId(eEvent.TenantId);
            await activeWxMessageBLL.ActiveWxMessageAddConsumerEvent(eEvent);
        }
    }
}
