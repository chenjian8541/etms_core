using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess
{
    public class DistributedLockDAL : IDistributedLockDAL
    {
        private readonly ICacheProvider _cacheProvider;

        public DistributedLockDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public bool LockTake<T>(T obj) where T : class, IRedisToken
        {
            return _cacheProvider.LockTake(obj.TenantId, obj.GetKey(), obj.GetKey(), obj.TimeOut);
        }

        public bool LockRelease<T>(T obj) where T : class, IRedisToken
        {
            return _cacheProvider.LockRelease(obj.TenantId, obj.GetKey());
        }
    }
}
