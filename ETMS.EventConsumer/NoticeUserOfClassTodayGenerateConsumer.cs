using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeUserOfClassTodayGenerateQueue")]
    public class NoticeUserOfClassTodayGenerateConsumer : ConsumerBase<NoticeUserOfClassTodayGenerateEvent>
    {
        protected override async Task Receive(NoticeUserOfClassTodayGenerateEvent eEvent)
        {
            var userSendNoticeBLL = CustomServiceLocator.GetInstance<IUserSendNoticeBLL>();
            userSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await userSendNoticeBLL.NoticeUserOfClassTodayGenerateConsumerEvent(eEvent);
        }
    }
}