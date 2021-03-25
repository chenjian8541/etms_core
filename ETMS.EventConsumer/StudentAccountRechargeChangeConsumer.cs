using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StudentAccountRechargeChangeQueue")]
    public class StudentAccountRechargeChangeConsumer : ConsumerBase<StudentAccountRechargeChangeEvent>
    {
        protected override async Task Receive(StudentAccountRechargeChangeEvent eEvent)
        {
            var studentAccountRechargeCoreBLL = CustomServiceLocator.GetInstance<IStudentAccountRechargeCoreBLL>();
            studentAccountRechargeCoreBLL.InitTenantId(eEvent.TenantId);
            await studentAccountRechargeCoreBLL.StudentAccountRechargeChange(eEvent);
        }
    }
}