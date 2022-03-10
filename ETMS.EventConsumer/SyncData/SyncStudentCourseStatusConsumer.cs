using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.SyncData
{
    [QueueConsumerAttribution("SyncStudentCourseStatusQueue")]
    public class SyncStudentCourseStatusConsumer : ConsumerBase<SyncStudentCourseStatusEvent>
    {
        protected override async Task Receive(SyncStudentCourseStatusEvent eEvent)
        {
            var evStudentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            evStudentBLL.InitTenantId(eEvent.TenantId);
            await evStudentBLL.SyncStudentCourseStatusConsumerEvent(eEvent);
        }
    }
}
