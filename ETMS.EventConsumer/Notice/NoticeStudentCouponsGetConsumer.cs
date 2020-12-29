using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentCouponsGetQueue")]
    public class NoticeStudentCouponsGetConsumer : ConsumerBase<NoticeStudentCouponsGetEvent>
    {
        protected override async Task Receive(NoticeStudentCouponsGetEvent eEvent)
        {
            var studentSendNotice3BLL = CustomServiceLocator.GetInstance<IStudentSendNotice3BLL>();
            studentSendNotice3BLL.InitTenantId(eEvent.TenantId);
            await studentSendNotice3BLL.NoticeStudentCouponsGetConsumerEvent(eEvent);
        }
    }
}