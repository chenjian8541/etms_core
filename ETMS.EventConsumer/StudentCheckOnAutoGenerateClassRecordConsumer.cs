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
    [QueueConsumerAttribution("StudentCheckOnAutoGenerateClassRecordQueue")]
    public class StudentCheckOnAutoGenerateClassRecordConsumer : ConsumerBase<StudentCheckOnAutoGenerateClassRecordEvent>
    {
        protected override async Task Receive(StudentCheckOnAutoGenerateClassRecordEvent eEvent)
        {
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            await evClassBLL.StudentCheckOnAutoGenerateClassRecordConsumerEvent(eEvent);
        }
    }
}
