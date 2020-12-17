using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EtmsManage;
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
    [QueueConsumerAttribution("TenantTxCloudUCountQueue")]

    public class TenantTxCloudUCountConsumer : ConsumerBase<TenantTxCloudUCountEvent>
    {
        private IDistributedLockDAL _distributedLockDAL;

        private ISysTenantTxCloudUCountBLL _sysTenantTxCloudUCountBLL;

        private TenantTxCloudUCountToken _lockKey;

        private int tryTimes = 1;

        protected override async Task Receive(TenantTxCloudUCountEvent eEvent)
        {
            _distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            _sysTenantTxCloudUCountBLL = CustomServiceLocator.GetInstance<ISysTenantTxCloudUCountBLL>();
            _sysTenantTxCloudUCountBLL.InitTenantId(eEvent.TenantId);
            _lockKey = new TenantTxCloudUCountToken(eEvent.TenantId);
            await Process(eEvent);
        }

        private async Task Process(TenantTxCloudUCountEvent eEvent)
        {
            if (_distributedLockDAL.LockTake(_lockKey))
            {
                try
                {
                    await _sysTenantTxCloudUCountBLL.TenantTxCloudUCountConsumerEvent(eEvent);
                }
                finally
                {
                    _distributedLockDAL.LockRelease(_lockKey);
                }
            }
            else
            {
                tryTimes++;
                if (tryTimes > 20)
                {
                    Log.Error($"【TenantTxCloudUCountConsumer】增加机构调用腾讯云人脸识别接口数量,参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
                    return;
                }
                System.Threading.Thread.Sleep(3000);  //等待三秒再执行
                await Process(eEvent);
            }
        }
    }
}
