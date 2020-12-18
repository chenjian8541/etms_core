using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfWxMessageQueue")]
    public class NoticeStudentsOfWxMessageConsumer : ConsumerBase<NoticeStudentsOfWxMessageEvent>
    {
        protected override async Task Receive(NoticeStudentsOfWxMessageEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNoticeBLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfWxMessageConsumerEvent(eEvent);
        }
    }
}
