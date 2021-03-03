using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StudentAccountRefundQueue")]
    public class StudentAccountRefundConsumer : ConsumerBase<StudentAccountRefundEvent>
    {
        protected override async Task Receive(StudentAccountRefundEvent request)
        {
            var studentAccountRechargeBLL = CustomServiceLocator.GetInstance<IStudentAccountRechargeBLL>();
            studentAccountRechargeBLL.InitTenantId(request.TenantId);
            await studentAccountRechargeBLL.StudentAccountRefundConsumerEvent(request);
        }
    }
}
