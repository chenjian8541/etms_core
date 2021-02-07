using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeUserOfStudentTryClassFinishQueue")]
    class NoticeUserOfStudentTryClassFinishConsumer : ConsumerBase<NoticeUserOfStudentTryClassFinishEvent>
    {
        protected override async Task Receive(NoticeUserOfStudentTryClassFinishEvent eEvent)
        {
            var userSendNoticeBLL = CustomServiceLocator.GetInstance<IUserSendNoticeBLL>();
            userSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await userSendNoticeBLL.NoticeUserOfStudentTryClassFinishConsumerEvent(eEvent);
        }
    }
}