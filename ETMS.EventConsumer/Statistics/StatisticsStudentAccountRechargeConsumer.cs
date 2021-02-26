using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IOC;
using ETMS.LOG;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Statistics
{
    [QueueConsumerAttribution("StatisticsStudentAccountRechargeQueue")]
    public class StatisticsStudentAccountRechargeConsumer : ConsumerBase<StatisticsStudentAccountRechargeEvent>
    {
        protected override async Task Receive(StatisticsStudentAccountRechargeEvent eEvent)
        {
            var statisticsTenantBLL = CustomServiceLocator.GetInstance<IStatisticsTenantBLL>();
            statisticsTenantBLL.InitTenantId(eEvent.TenantId);
            var lockKey = new StatisticsStudentAccountRechargeToken(eEvent.TenantId);
            var distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            if (distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    await statisticsTenantBLL.StatisticsStudentAccountRechargeConsumerEvent(eEvent);
                }
                finally
                {
                    distributedLockDAL.LockRelease(lockKey);
                }
            }
            else
            {
                Log.Debug($"【StatisticsStudentAccountRechargeConsumer】丢弃请求,参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
            }
        }
    }
}
