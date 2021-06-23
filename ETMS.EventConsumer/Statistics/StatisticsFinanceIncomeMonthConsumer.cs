using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Statistics
{
    [QueueConsumerAttribution("StatisticsFinanceIncomeMonthQueue")]
    public class StatisticsFinanceIncomeMonthConsumer : ConsumerBase<StatisticsFinanceIncomeMonthEvent>
    {
        protected override async Task Receive(StatisticsFinanceIncomeMonthEvent eEvent)
        {
            var _lockKey = new StatisticsFinanceIncomeMonthToken(eEvent.TenantId);
            var financeIncomeBLL = CustomServiceLocator.GetInstance<IFinanceIncomeBLL>();
            financeIncomeBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsFinanceIncomeMonthToken>(_lockKey, eEvent, this.ClassName,
                async () =>
                await financeIncomeBLL.StatisticsFinanceIncomeMonthConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
