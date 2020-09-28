using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsStudentTrackCountQueue")]
    public class StatisticsStudentTrackCountConsumer : ConsumerBase<StatisticsStudentTrackCountEvent>
    {
        protected override async Task Receive(StatisticsStudentTrackCountEvent eEvent)
        {
            var statisticsStudentBLL = CustomServiceLocator.GetInstance<IStatisticsStudentBLL>();
            statisticsStudentBLL.InitTenantId(eEvent.TenantId);
            await statisticsStudentBLL.StatisticsStudentTrackCountConsumeEvent(eEvent);
        }
    }
}
