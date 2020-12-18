using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfHomeworkAddCommentQueue")]
    public class NoticeStudentsOfHomeworkAddCommentEventConsumer : ConsumerBase<NoticeStudentsOfHomeworkAddCommentEvent>
    {
        protected override async Task Receive(NoticeStudentsOfHomeworkAddCommentEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfHomeworkAddCommentConsumeEvent(eEvent);
        }
    }
}
