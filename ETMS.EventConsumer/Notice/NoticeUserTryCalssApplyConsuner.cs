using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeUserTryCalssApplyQueue")]
    public class NoticeUserTryCalssApplyConsuner : ConsumerBase<NoticeUserTryCalssApplyEvent>
    {
        protected override async Task Receive(NoticeUserTryCalssApplyEvent eEvent)
        {
            var userSendNoticeBLL = CustomServiceLocator.GetInstance<IUserSendNoticeBLL>();
            userSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await userSendNoticeBLL.NoticeUserTryCalssApplyConsumerEvent(eEvent);
        }
    }
}