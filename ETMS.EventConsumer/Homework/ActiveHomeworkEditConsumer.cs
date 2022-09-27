using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ActiveHomeworkEditQueue")]
    public class ActiveHomeworkEditConsumer : ConsumerBase<ActiveHomeworkEditEvent>
    {
        protected override async Task Receive(ActiveHomeworkEditEvent eEvent)
        {
            var evHomeworkBLL = CustomServiceLocator.GetInstance<IEvHomeworkBLL>();
            evHomeworkBLL.InitTenantId(eEvent.TenantId);
            await evHomeworkBLL.ActiveHomeworkEditConsumerEvent(eEvent);
        }
    }
}
