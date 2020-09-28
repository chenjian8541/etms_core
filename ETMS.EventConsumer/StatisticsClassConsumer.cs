using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsClassQueue")]
    public class StatisticsClassConsumer : ConsumerBase<StatisticsClassEvent>
    {
        protected override async Task Receive(StatisticsClassEvent eEvent)
        {
            var statisticsClassBLL = CustomServiceLocator.GetInstance<IStatisticsClassBLL>();
            statisticsClassBLL.InitTenantId(eEvent.TenantId);
            await statisticsClassBLL.StatisticsClassConsumeEvent(eEvent);
        }
    }
}
