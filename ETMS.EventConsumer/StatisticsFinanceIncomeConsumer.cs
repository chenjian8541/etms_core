using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsFinanceIncomeQueue")]
    public class StatisticsFinanceIncomeConsumer : ConsumerBase<StatisticsFinanceIncomeEvent>
    {
        protected override async Task Receive(StatisticsFinanceIncomeEvent eEvent)
        {
            var statisticsFinanceBLL = CustomServiceLocator.GetInstance<IStatisticsFinanceBLL>();
            statisticsFinanceBLL.InitTenantId(eEvent.TenantId);
            await statisticsFinanceBLL.StatisticsFinanceIncomeConsumeEvent(eEvent);
        }
    }
}
