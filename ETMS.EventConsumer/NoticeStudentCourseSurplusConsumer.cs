using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentCourseSurplusQueue")]
    public class NoticeStudentCourseSurplusConsumer : ConsumerBase<NoticeStudentCourseSurplusEvent>
    {
        protected override async Task Receive(NoticeStudentCourseSurplusEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentCourseSurplusConsumerEvent(eEvent);
        }
    }
}