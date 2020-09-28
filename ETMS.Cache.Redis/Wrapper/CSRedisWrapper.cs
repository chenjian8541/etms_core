using CSRedis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Cache.Redis.Wrapper
{
    public class CSRedisWrapper
    {
        private static int _dbCount;

        private static CSRedisClient[] _cSRedisClients;

        public static void Initialize(string strConFormat, int dbCount)
        {
            _dbCount = dbCount;
            _cSRedisClients = new CSRedisClient[dbCount];
            for (var i = 0; i < dbCount; i++)
            {
                _cSRedisClients[i] = new CSRedisClient(string.Format(strConFormat, i));
            }
        }

        internal static bool Set(int tenantId, string key, string t)
        {
            return _cSRedisClients[tenantId % _dbCount].Set(key, t);
        }

        internal static bool Set(int tenantId, string key, string t, TimeSpan timeSpan)
        {
            return _cSRedisClients[tenantId % _dbCount].Set(key, t, timeSpan);
        }

        internal static string Get(int tenantId, string key)
        {
            return _cSRedisClients[tenantId % _dbCount].Get(key);
        }

        internal static bool Remove(int tenantId, string key)
        {
            return _cSRedisClients[tenantId % _dbCount].Del(key) > 0;
        }

        internal static bool LockTake(int tenantId, string Key, string value, TimeSpan timeSpan)
        {
            var clent = _cSRedisClients[tenantId % _dbCount];
            if (clent.SetNx(Key, value))
            {
                clent.Expire(Key, timeSpan);
                return true;
            }
            return false;
        }

        internal static bool LockRelease(int tenantId, string key)
        {
            return _cSRedisClients[tenantId % _dbCount].Del(key) > 0;
        }
    }
}
