using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeUserContractsNotArrivedQueue")]
    public class NoticeUserContractsNotArrivedConsumer : ConsumerBase<NoticeUserContractsNotArrivedEvent>
    {
        protected override async Task Receive(NoticeUserContractsNotArrivedEvent eEvent)
        {
            var userSendNoticeBLL = CustomServiceLocator.GetInstance<IUserSendNoticeBLL>();
            userSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await userSendNoticeBLL.NoticeUserContractsNotArrivedConsumerEvent(eEvent);
        }
    }
}
