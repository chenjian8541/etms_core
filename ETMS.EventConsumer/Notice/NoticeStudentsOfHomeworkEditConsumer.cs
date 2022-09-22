using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Notice
{
    [QueueConsumerAttribution("NoticeStudentsOfHomeworkEditQueue")]
    public class NoticeStudentsOfHomeworkEditConsumer: ConsumerBase<NoticeStudentsOfHomeworkEditEvent>
    {
        protected override async Task Receive(NoticeStudentsOfHomeworkEditEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfHomeworkEditConsumeEvent(eEvent);
        }
    }
}
