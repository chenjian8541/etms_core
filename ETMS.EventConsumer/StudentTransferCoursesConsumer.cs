using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StudentTransferCoursesQueue")]
    public class StudentTransferCoursesConsumer : ConsumerBase<StudentTransferCoursesEvent>
    {
        protected override async Task Receive(StudentTransferCoursesEvent eEvent)
        {
            var studentTransferCourses = CustomServiceLocator.GetInstance<IStudentTransferCourses>();
            studentTransferCourses.InitTenantId(eEvent.TenantId);
            await studentTransferCourses.StudentTransferCoursesConsumerEvent(eEvent);
        }
    }
}
