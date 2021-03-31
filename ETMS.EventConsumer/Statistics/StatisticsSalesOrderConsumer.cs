using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IOC;
using ETMS.LOG;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StatisticsSalesOrderQueue")]
    public class StatisticsSalesOrderConsumer : ConsumerBase<StatisticsSalesOrderEvent>
    {
        protected override async Task Receive(StatisticsSalesOrderEvent eEvent)
        {
            var _lockKey = new StatisticsSalesOrderConsumerToken(eEvent.TenantId);
            var _statisticsSalesBLL = CustomServiceLocator.GetInstance<IStatisticsSalesBLL>();
            _statisticsSalesBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<StatisticsSalesOrderConsumerToken>(_lockKey, eEvent, this.ClassName,
                async () =>
                    await _statisticsSalesBLL.StatisticsSalesOrderConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
