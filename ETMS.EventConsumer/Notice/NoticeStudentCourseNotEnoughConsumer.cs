using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentCourseNotEnoughQueue")]
    public class NoticeStudentCourseNotEnoughConsumer : ConsumerBase<NoticeStudentCourseNotEnoughEvent>
    {
        protected override async Task Receive(NoticeStudentCourseNotEnoughEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentCourseNotEnoughConsumerEvent(eEvent);
        }
    }
}