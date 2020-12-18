using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsStudentCountQueue")]
    public class StatisticsStudentCountConsumer : ConsumerBase<StatisticsStudentCountEvent>
    {
        protected override async Task Receive(StatisticsStudentCountEvent eEvent)
        {
            var statisticsStudentBLL = CustomServiceLocator.GetInstance<IStatisticsStudentBLL>();
            statisticsStudentBLL.InitTenantId(eEvent.TenantId);
            await statisticsStudentBLL.StatisticsStudentCountConsumeEvent(eEvent);
        }
    }
}
