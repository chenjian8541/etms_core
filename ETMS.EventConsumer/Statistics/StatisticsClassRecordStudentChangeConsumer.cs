using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Statistics
{
    [QueueConsumerAttribution("StatisticsClassRecordStudentChangeQueue")]
    public class StatisticsClassRecordStudentChangeConsumer : ConsumerBase<StatisticsClassRecordStudentChangeEvent>
    {
        protected override async Task Receive(StatisticsClassRecordStudentChangeEvent eEvent)
        {
            var statisticsClassBLL = CustomServiceLocator.GetInstance<IStatisticsClassBLL>();
            statisticsClassBLL.InitTenantId(eEvent.TenantId);
            await statisticsClassBLL.StatisticsClassRecordStudentChangeConsumeEvent(eEvent);
        }
    }
}
