using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("EveryDayStatisticsQueue")]
    public class EveryDayStatisticsConsumer : ConsumerBase<EveryDayStatisticsEvent>
    {
        protected override async Task Receive(EveryDayStatisticsEvent @event)
        {
            var userSendNoticeBLL = CustomServiceLocator.GetInstance<IUserSendNoticeBLL>();
            userSendNoticeBLL.InitTenantId(@event.TenantId);
            await userSendNoticeBLL.EveryDayStatisticsConsumerEvent(@event);
        }
    }
}
