using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsCheckSignQueue")]
    public class NoticeStudentsCheckSignConsumer : ConsumerBase<NoticeStudentsCheckSignEvent>
    {
        protected override async Task Receive(NoticeStudentsCheckSignEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNoticeBLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsCheckSign(eEvent);
        }
    }
}
