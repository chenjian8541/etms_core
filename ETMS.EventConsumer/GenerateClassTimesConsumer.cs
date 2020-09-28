using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
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
    [QueueConsumerAttribution("GenerateClassTimesQueue")]
    public class GenerateClassTimesConsumer : ConsumerBase<GenerateClassTimesEvent>
    {
        protected override async Task Receive(GenerateClassTimesEvent eEvent)
        {
            var analyzeClassTimesBLL = CustomServiceLocator.GetInstance<IJobAnalyzeBLL>();
            analyzeClassTimesBLL.InitTenantId(eEvent.TenantId);
            var lockKey = new GenerateClassTimesToken(eEvent.TenantId, eEvent.ClassTimesRuleId);
            var distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            if (distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    await analyzeClassTimesBLL.GenerateClassTimesEvent(eEvent);
                }
                finally
                {
                    distributedLockDAL.LockRelease(lockKey);
                }
            }
            else
            {
                Log.Debug($"【GenerateClassTimesConsumer】丢弃请求,参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
            }
        }
    }
}
