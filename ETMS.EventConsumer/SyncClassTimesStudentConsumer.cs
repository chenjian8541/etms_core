using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncClassTimesStudentQueue")]
    public class SyncClassTimesStudentConsumer : ConsumerBase<SyncClassTimesStudentEvent>
    {
        protected override async Task Receive(SyncClassTimesStudentEvent eEvent)
        {
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            await evClassBLL.SyncClassTimesStudentConsumerEvent(eEvent);
        }
    }
}
