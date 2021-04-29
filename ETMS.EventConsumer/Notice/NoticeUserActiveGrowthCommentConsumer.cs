using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Notice
{
    [QueueConsumerAttribution("NoticeUserActiveGrowthCommentQueue")]
    public class NoticeUserActiveGrowthCommentConsumer : ConsumerBase<NoticeUserActiveGrowthCommentEvent>
    {
        protected override async Task Receive(NoticeUserActiveGrowthCommentEvent eEvent)
        {
            var userSendNotice2BLL = CustomServiceLocator.GetInstance<IUserSendNotice2BLL>();
            userSendNotice2BLL.InitTenantId(eEvent.TenantId);
            await userSendNotice2BLL.NoticeUserActiveGrowthCommentConsumerEvent(eEvent);
        }
    }
}
