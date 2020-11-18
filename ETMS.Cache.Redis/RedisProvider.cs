using Newtonsoft.Json;
using System;
using ETMS.ICache;
using ETMS.IOC;
using ETMS.Cache.Redis.Wrapper;

namespace ETMS.Cache.Redis
{
    /// <summary>
    /// 使用Redis实现缓存服务
    /// </summary>
    public class RedisProvider : ICacheProvider
    {
        public bool Set<T>(int tenantId, string key, T t) where T : class
        {
            return CSRedisWrapper.Set(tenantId, key, JsonConvert.SerializeObject(t));
        }

        public bool Set<T>(int tenantId, string key, T t, TimeSpan timeSpan) where T : class
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return Set(tenantId, key, t);
            }
            return CSRedisWrapper.Set(tenantId, key, JsonConvert.SerializeObject(t), timeSpan);
        }

        public T Get<T>(int tenantId, string key) where T : class
        {
            var s = CSRedisWrapper.Get(tenantId, key);
            if (string.IsNullOrEmpty(s))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(s);
        }

        public bool Remove(int tenantId, string key)
        {
            return CSRedisWrapper.Remove(tenantId, key);
        }

        public bool Remove(int tenantId, string[] keys)
        {
            return CSRedisWrapper.Remove(tenantId, keys);
        }

        public bool LockTake(int tenantId, string Key, string value, TimeSpan expiry)
        {
            return CSRedisWrapper.LockTake(tenantId, Key, value, expiry);
        }

        public bool LockRelease(int tenantId, string key)
        {
            return CSRedisWrapper.LockRelease(tenantId, key);
        }
    }
}
