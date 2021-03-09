using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentAccountRechargeChangedQueue")]
    public class StudentAccountRechargeChangedConsumer : ConsumerBase<NoticeStudentAccountRechargeChangedEvent>
    {
        protected override async Task Receive(NoticeStudentAccountRechargeChangedEvent eEvent)
        {
            var studentSendNotice3BLL = CustomServiceLocator.GetInstance<IStudentSendNotice3BLL>();
            studentSendNotice3BLL.InitTenantId(eEvent.TenantId);
            await studentSendNotice3BLL.NoticeStudentAccountRechargeChangedConsumerEvent(eEvent);
        }
    }
}