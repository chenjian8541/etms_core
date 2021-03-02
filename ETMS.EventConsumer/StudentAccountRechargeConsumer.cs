using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StudentAccountRechargeQueue")]
    public class StudentAccountRechargeConsumer : ConsumerBase<StudentAccountRechargeEvent>
    {
        protected override async Task Receive(StudentAccountRechargeEvent request)
        {
            var studentAccountRechargeBLL = CustomServiceLocator.GetInstance<IStudentAccountRechargeBLL>();
            studentAccountRechargeBLL.InitTenantId(request.TenantId);
            await studentAccountRechargeBLL.StudentAccountRechargeConsumerEvent(request);
        }
    }
}
