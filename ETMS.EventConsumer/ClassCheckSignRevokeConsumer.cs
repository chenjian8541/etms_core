using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ClassCheckSignRevokeQueue")]
    public class ClassCheckSignRevokeConsumer : ConsumerBase<ClassCheckSignRevokeEvent>
    {
        protected override async Task Receive(ClassCheckSignRevokeEvent eEvent)
        {
            var classCheckSignRevokeBLL = CustomServiceLocator.GetInstance<IClassCheckSignRevokeBLL>();
            classCheckSignRevokeBLL.InitTenantId(eEvent.TenantId);
            await classCheckSignRevokeBLL.ClassCheckSignRevokeEvent(eEvent);
        }
    }
}

