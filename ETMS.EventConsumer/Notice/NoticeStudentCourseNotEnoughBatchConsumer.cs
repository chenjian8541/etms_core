using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Notice
{
    [QueueConsumerAttribution("NoticeStudentCourseNotEnoughBatchQueue")]
    public class NoticeStudentCourseNotEnoughBatchConsumer : ConsumerBase<NoticeStudentCourseNotEnoughBatchEvent>
    {
        protected override async Task Receive(NoticeStudentCourseNotEnoughBatchEvent eEvent)
        {
            var studentSendNotice2BLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNotice2BLL.InitTenantId(eEvent.TenantId);
            studentSendNotice2BLL.NoticeStudentCourseNotEnoughBatchConsumerEvent(eEvent);
        }
    }
}
