using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsStudentQueue")]
    public class StatisticsStudentConsumer : ConsumerBase<StatisticsStudentEvent>
    {
        protected override async Task Receive(StatisticsStudentEvent eEvent)
        {
            var statisticsStudentBLL = CustomServiceLocator.GetInstance<IStatisticsStudentBLL>();
            statisticsStudentBLL.InitTenantId(eEvent.TenantId);
            await statisticsStudentBLL.StatisticsStudentConsumeEvent(eEvent);
        }
    }
}
