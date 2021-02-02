using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ClassOfOneAutoOverQueue")]
    public class ClassOfOneAutoOverConsumer : ConsumerBase<ClassOfOneAutoOverEvent>
    {
        protected override async Task Receive(ClassOfOneAutoOverEvent eEvent)
        {
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            await evClassBLL.ClassOfOneAutoOverConsumerEvent(eEvent);
        }
    }
}