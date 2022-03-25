using ETMS.Entity.CacheBucket.Alien;
using ETMS.ICache;
using ETMS.IDataAccess.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Alien
{
    public class MgTempDataCacheDAL : IMgTempDataCacheDAL
    {
        private readonly ICacheProvider _cacheProvider;

        public MgTempDataCacheDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public MgUserLoginOnlineBucket GetUserLoginOnlineBucket(int headId, long userId, int loginClientType)
        {
            return _cacheProvider.Get<MgUserLoginOnlineBucket>(0, new MgUserLoginOnlineBucket().GetKeyFormat(headId, userId, loginClientType));
        }

        public void SetUserLoginOnlineBucket(int headId, long userId, string loginTime, int loginClientType)
        {
            var bucket = new MgUserLoginOnlineBucket()
            {
                LoginTime = loginTime,
                HeadId = headId,
                UserId = userId,
                LoginClientType = loginClientType
            };
            var key = bucket.GetKeyFormat(headId, userId, loginClientType);
            _cacheProvider.Set(0, key, bucket, bucket.TimeOut);
        }
    }
}
