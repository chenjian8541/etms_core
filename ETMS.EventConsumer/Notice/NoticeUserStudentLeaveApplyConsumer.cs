using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeUserStudentLeaveApplyQueue")]
    public class NoticeUserStudentLeaveApplyConsumer : ConsumerBase<NoticeUserStudentLeaveApplyEvent>
    {
        protected override async Task Receive(NoticeUserStudentLeaveApplyEvent eEvent)
        {
            var userSendNoticeBLL = CustomServiceLocator.GetInstance<IUserSendNoticeBLL>();
            userSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await userSendNoticeBLL.NoticeUserStudentLeaveApplyConsumerEvent(eEvent);
        }
    }
}