using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Notice
{
    [QueueConsumerAttribution("NoticeManageQueue")]
    public class NoticeManageConsumer : ConsumerBase<NoticeManageEvent>
    {
        protected override async Task Receive(NoticeManageEvent eEvent)
        {
            var noticeManageBLL = CustomServiceLocator.GetInstance<INoticeManageBLL>();
            await noticeManageBLL.NoticeManageConsumerEvent(eEvent);
        }
    }
}
