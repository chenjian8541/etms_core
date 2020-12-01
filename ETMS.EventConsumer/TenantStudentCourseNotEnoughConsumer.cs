using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("TenantStudentCourseNotEnoughQueue")]
    public class TenantStudentCourseNotEnoughConsumer : ConsumerBase<TenantStudentCourseNotEnoughEvent>
    {
        protected override async Task Receive(TenantStudentCourseNotEnoughEvent eEvent)
        {
            var studentCourseAnalyzeBLL = CustomServiceLocator.GetInstance<IStudentCourseAnalyzeBLL>();
            studentCourseAnalyzeBLL.InitTenantId(eEvent.TenantId);
            await studentCourseAnalyzeBLL.TenantStudentCourseNotEnoughConsumerEvent(eEvent);
        }
    }
}
