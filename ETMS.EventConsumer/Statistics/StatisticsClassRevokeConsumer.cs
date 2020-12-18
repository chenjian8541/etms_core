using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsClassRevokeEventQueue")]
    public class StatisticsClassRevokeConsumer : ConsumerBase<StatisticsClassRevokeEvent>
    {
        protected override async Task Receive(StatisticsClassRevokeEvent eEvent)
        {
            var statisticsClassBLL = CustomServiceLocator.GetInstance<IStatisticsClassBLL>();
            statisticsClassBLL.InitTenantId(eEvent.TenantId);
            await statisticsClassBLL.StatisticsClassRevokeConsumeEvent(eEvent);
        }
    }
}
