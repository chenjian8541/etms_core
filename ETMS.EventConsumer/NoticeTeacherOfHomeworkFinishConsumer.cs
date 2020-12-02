using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeTeacherOfHomeworkFinishQueue")]
    public class NoticeTeacherOfHomeworkFinishConsumer : ConsumerBase<NoticeTeacherOfHomeworkFinishEvent>
    {
        protected override async Task Receive(NoticeTeacherOfHomeworkFinishEvent eEvent)
        {
            var userSendNoticeBLL = CustomServiceLocator.GetInstance<IUserSendNoticeBLL>();
            userSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await userSendNoticeBLL.NoticeTeacherOfHomeworkFinishConsumerEvent(eEvent);
        }
    }
}