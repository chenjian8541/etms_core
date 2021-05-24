﻿using ETMS.Entity.Config;
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
    public class LockTakeHandler<T> where T : class, IRedisToken
    {
        private IDistributedLockDAL _distributedLockDAL;

        private T _lockKey;

        private Func<Task> _process;

        private ETMS.Event.DataContract.Event _request;

        private string _processName;

        private IEventPublisher _eventPublisher;

        public LockTakeHandler(T lockKey, ETMS.Event.DataContract.Event request, string processName, Func<Task> process)
        {
            this._lockKey = lockKey;
            this._process = process;
            this._request = request;
            this._processName = processName;
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
