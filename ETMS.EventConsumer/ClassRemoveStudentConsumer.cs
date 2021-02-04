using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ClassRemoveStudentQueue")]
    public class ClassRemoveStudentConsumer : ConsumerBase<ClassRemoveStudentEvent>
    {
        protected override async Task Receive(ClassRemoveStudentEvent eEvent)
        {
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            await evClassBLL.ClassRemoveStudentConsumerEvent(eEvent);
        }
    }
}
