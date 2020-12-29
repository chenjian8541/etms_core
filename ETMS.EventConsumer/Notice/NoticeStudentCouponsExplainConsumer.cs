using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentCouponsExplainQueue")]
    public class NoticeStudentCouponsExplainConsumer : ConsumerBase<NoticeStudentCouponsExplainEvent>
    {
        protected override async Task Receive(NoticeStudentCouponsExplainEvent eEvent)
        {
            var studentSendNotice3BLL = CustomServiceLocator.GetInstance<IStudentSendNotice3BLL>();
            studentSendNotice3BLL.InitTenantId(eEvent.TenantId);
            await studentSendNotice3BLL.NoticeStudentCouponsExplainConsumetEvent(eEvent);
        }
    }
}