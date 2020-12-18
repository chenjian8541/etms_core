using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfMakeupQueue")]
    public class NoticeStudentsOfMakeupConsumer : ConsumerBase<NoticeStudentsOfMakeupEvent>
    {
        protected override async Task Receive(NoticeStudentsOfMakeupEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfMakeupConsumerEvent(eEvent);
        }
    }
}