using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsSalesCourseQueue")]
    public class StatisticsSalesCourseConsumer : ConsumerBase<StatisticsSalesCourseEvent>
    {
        protected override async Task Receive(StatisticsSalesCourseEvent eEvent)
        {
            var statisticsSalesCourseBLL = CustomServiceLocator.GetInstance<IStatisticsSalesCourseBLL>();
            statisticsSalesCourseBLL.InitTenantId(eEvent.TenantId);
            await statisticsSalesCourseBLL.StatisticsSalesCourseConsumeEvent(eEvent);
        }
    }
}
