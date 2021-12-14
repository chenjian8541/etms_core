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
    [QueueConsumerAttribution("StudentCourseMarkExceedQueue")]
    public class StudentCourseMarkExceedConsumer : ConsumerBase<StudentCourseMarkExceedEvent>
    {
        protected override async Task Receive(StudentCourseMarkExceedEvent request)
        {
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(request.TenantId);
            await evClassBLL.StudentCourseMarkExceedConsumerEvent(request);
        }
    }
}
