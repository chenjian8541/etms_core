using ETMS.Entity.Config;
using ETMS.Event.DataContract;
using ETMS.IDataAccess;
using ETMS.IOC;
using ETMS.LOG;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Lib
{
    /// <summary>
    /// 分布式事务锁_执行程序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LockTakeHandler<T> where T : class, IRedisToken
    {
        private IDistributedLockDAL _distributedLockDAL;

        private int tryTimes = 1;

        private readonly T _lockKey;

        private Func<Task> _process;

        private IEvent _request;

        private readonly string _processName;

        public LockTakeHandler(T lockKey, IEvent request, string processName, Func<Task> process)
        {
            this._lockKey = lockKey;
            this._process = process;
            this._request = request;
            this._processName = processName;
            _distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
        }

        public async Task Process()
        {
            await Run();
        }

        private async Task Run()
        {
            if (_distributedLockDAL.LockTake(_lockKey))
            {
                try
                {
                    await this._process.Invoke();
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
                    Log.Error($"【{_processName}】尝试了50次仍未获得执行锁,参数:{JsonConvert.SerializeObject(_request)}", this.GetType());
                    return;
                }
                System.Threading.Thread.Sleep(3000);  //等待三秒再执行
                await Run();
            }
        }
    }
}
