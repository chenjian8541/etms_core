using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IDataAccess
{
    public interface IDistributedLockDAL
    {
        bool LockTake<T>(T obj) where T : class, IRedisToken;

        bool LockRelease<T>(T obj) where T : class, IRedisToken;
    }
}
