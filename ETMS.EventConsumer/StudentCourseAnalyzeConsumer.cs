using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StudentCourseAnalyzeQueue")]
    public class StudentCourseAnalyzeConsumer : ConsumerBase<StudentCourseAnalyzeEvent>
    {
        protected override async Task Receive(StudentCourseAnalyzeEvent eEvent)
        {
            var studentCourseAnalyzeBLL = CustomServiceLocator.GetInstance<IStudentCourseAnalyzeBLL>();
            studentCourseAnalyzeBLL.InitTenantId(eEvent.TenantId);
            await studentCourseAnalyzeBLL.CourseAnalyze(eEvent);
        }
    }
}
