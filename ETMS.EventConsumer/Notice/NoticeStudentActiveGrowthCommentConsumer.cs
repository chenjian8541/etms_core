using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Notice
{
    [QueueConsumerAttribution("NoticeStudentActiveGrowthCommentQueue")]
    public class NoticeStudentActiveGrowthCommentConsumer: ConsumerBase<NoticeStudentActiveGrowthCommentEvent>
    {
        protected override async Task Receive(NoticeStudentActiveGrowthCommentEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice3BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentActiveGrowthCommentConsumerEvent(eEvent);
        }
    }
}
