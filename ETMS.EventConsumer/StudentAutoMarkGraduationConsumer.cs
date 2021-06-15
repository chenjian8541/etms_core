using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StudentAutoMarkGraduationQueue")]
    public class StudentAutoMarkGraduationConsumer : ConsumerBase<StudentAutoMarkGraduationEvent>
    {
        protected override async Task Receive(StudentAutoMarkGraduationEvent eEvent)
        {
            var evStudentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            evStudentBLL.InitTenantId(eEvent.TenantId);
            await evStudentBLL.StudentAutoMarkGraduationConsumerEvent(eEvent);
        }
    }
}