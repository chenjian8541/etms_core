using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentClassCheckSignRevokeQueue")]
    public class NoticeStudentClassCheckSignRevokeConsumer : ConsumerBase<NoticeStudentClassCheckSignRevokeEvent>
    {
        protected override async Task Receive(NoticeStudentClassCheckSignRevokeEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice3BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentClassCheckSignRevokeConsumerEvent(eEvent);
        }
    }
}