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
    [QueueConsumerAttribution("SyncStudentLogOfSurplusCourseQueue")]
    public class SyncStudentLogOfSurplusCourseConsumer : ConsumerBase<SyncStudentLogOfSurplusCourseEvent>
    {
        protected override async Task Receive(SyncStudentLogOfSurplusCourseEvent eEvent)
        {
            var evEducationBLL = CustomServiceLocator.GetInstance<IEvEducationBLL>();
            evEducationBLL.InitTenantId(eEvent.TenantId);
            await evEducationBLL.SyncStudentLogOfSurplusCourseConsumerEvent(eEvent);
        }
    }
}
