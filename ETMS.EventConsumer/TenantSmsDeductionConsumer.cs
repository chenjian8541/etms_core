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
    [QueueConsumerAttribution("TenantSmsDeductionQueue")]
    public class TenantSmsDeductionConsumer : ConsumerBase<TenantSmsDeductionEvent>
    {
        private IDistributedLockDAL _distributedLockDAL;

        private ITenantBLL _tenantBLL;

        private TenantSmsDeductionToken _lockKey;

        private int tryTimes = 1;

        protected override async Task Receive(TenantSmsDeductionEvent eEvent)
        {
            _distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            _lockKey = new TenantSmsDeductionToken(eEvent.TenantId);
            _tenantBLL = CustomServiceLocator.GetInstance<ITenantBLL>();
            _tenantBLL.InitTenantId(eEvent.TenantId);
            await Process(eEvent);
        }

        private async Task Process(TenantSmsDeductionEvent eEvent)
        {
            if (_distributedLockDAL.LockTake(_lockKey))
            {
                try
                {
                    await _tenantBLL.TenantSmsDeductionEventConsume(eEvent);
                }
                finally
                {
                    _distributedLockDAL.LockRelease(_lockKey);
                }
            }
            else
            {
                tryTimes++;
                if (tryTimes > 50)
                {
                    Log.Error($"【TenantSmsDeductionConsumer】扣减机构短信数量，尝试了50次仍未获得执行锁,参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
                    return;
                }
                System.Threading.Thread.Sleep(3000);  //等待三秒再执行
                await Process(eEvent);
            }
        }
    }
}
