using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentLeaveApplyQueue")]
    public class NoticeStudentLeaveApplyConsumer : ConsumerBase<NoticeStudentLeaveApplyEvent>
    {
        protected override async Task Receive(NoticeStudentLeaveApplyEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNoticeBLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentLeaveApply(eEvent);
        }
    }
}
