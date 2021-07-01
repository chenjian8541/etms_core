using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
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
            var _lockKey = new StatisticsFinanceIncomeToken(eEvent.TenantId);
            var statisticsFinanceBLL = CustomServiceLocator.GetInstance<IStatisticsFinanceBLL>();
            statisticsFinanceBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsFinanceIncomeToken, StatisticsFinanceIncomeEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await statisticsFinanceBLL.StatisticsFinanceIncomeConsumeEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
