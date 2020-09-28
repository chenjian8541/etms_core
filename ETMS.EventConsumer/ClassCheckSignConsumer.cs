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
    [QueueConsumerAttribution("ClassCheckSignQueue")]
    public class ClassCheckSignConsumer : ConsumerBase<ClassCheckSignEvent>
    {
        private IClassCheckSignBLL _classCheckSignBLL;

        private IDistributedLockDAL _distributedLockDAL;

        private ClassCheckSignToken _lockKey;

        private int tryTimes = 1;

        protected override async Task Receive(ClassCheckSignEvent eEvent)
        {
            _classCheckSignBLL = CustomServiceLocator.GetInstance<IClassCheckSignBLL>();
            _classCheckSignBLL.InitTenantId(eEvent.TenantId);
            _lockKey = new ClassCheckSignToken(eEvent.TenantId);
            _distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            await Process(eEvent);
        }

        private async Task Process(ClassCheckSignEvent eEvent)
        {
            if (_distributedLockDAL.LockTake(_lockKey))
            {
                try
                {
                    await _classCheckSignBLL.ClassCheckSignEventProcessEvent(eEvent);
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
                    Log.Error($"【ClassCheckSignConsumer】点名，尝试了50次仍未获得执行锁,参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
                    return;
                }
                System.Threading.Thread.Sleep(3000);  //等待三秒再执行
                await Process(eEvent);
            }
        }
    }
}
