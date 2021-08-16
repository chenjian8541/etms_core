using ETMS.Entity.Config;
using ETMS.Event.DataContract;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
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
    public class LockTakeHandler<T, U> where T : class, IRedisToken where U : ETMS.Event.DataContract.Event
    {
        private IDistributedLockDAL _distributedLockDAL;

        private T _lockKey;

        private Func<Task> _process;

        private U _request;

        private string _processName;

        private IEventPublisher _eventPublisher;

        private bool _isErrCanTryManyTime;

        private const int MaxErrTryCount = 10;

        public LockTakeHandler(T lockKey, U request, string processName, Func<Task> process, bool isErrCanTryManyTime = true)
        {
            this._lockKey = lockKey;
            this._process = process;
            this._request = request;
            this._processName = processName;
            this._isErrCanTryManyTime = isErrCanTryManyTime;
            _distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            _eventPublisher = CustomServiceLocator.GetInstance<IEventPublisher>();
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
                catch (Exception ex)
                {
                    if (_isErrCanTryManyTime)
                    {
                        //执行失败后 是否可以再次尝试执行 等待5秒再执行
                        if (_request.ErrTryCount < MaxErrTryCount)
                        {
                            System.Threading.Thread.Sleep(5000);
                            _request.ErrTryCount++;
                            _eventPublisher.Publish(_request);
                        }
                    }
                    LOG.Log.Error($"[{_processName}]执行出错", ex, this.GetType());
                    throw;
                }
                finally
                {
                    _distributedLockDAL.LockRelease(_lockKey);
                }
            }
            else
            {
                _request.TryCount++;
                Log.Warn($"【{_processName}】第{_request.TryCount}失败尝试，仍未获得执行锁,参数:{JsonConvert.SerializeObject(_request)}", this.GetType());
                if (_request.TryCount > 100)
                {
                    Log.Error($"【{_processName}】尝试了100次仍未获得执行锁,参数:{JsonConvert.SerializeObject(_request)}", this.GetType());
                    _distributedLockDAL.LockRelease(_lockKey);
                }
                System.Threading.Thread.Sleep(3000);  //等待三秒再执行
                _eventPublisher.Publish(_request);
            }
        }
    }
}
