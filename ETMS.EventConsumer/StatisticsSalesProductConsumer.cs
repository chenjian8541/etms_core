using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsSalesProductQueue")]
    public class StatisticsSalesProductConsumer : ConsumerBase<StatisticsSalesProductEvent>
    {
        protected override async Task Receive(StatisticsSalesProductEvent eEvent)
        {
            var statisticsSalesBLL = CustomServiceLocator.GetInstance<IStatisticsSalesBLL>();
            statisticsSalesBLL.InitTenantId(eEvent.TenantId);
            await statisticsSalesBLL.StatisticsSalesProductConsumeEvent(eEvent);
        }
    }
}
