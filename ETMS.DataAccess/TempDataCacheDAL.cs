using ETMS.Entity.CacheBucket.TempShortTime;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess
{
    public class TempDataCacheDAL : ITempDataCacheDAL
    {
        private readonly ICacheProvider _cacheProvider;

        public TempDataCacheDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public NoticeStudentsOfClassDayBucket GetNoticeStudentsOfClassDayBucket(int tenantId, DateTime classOt)
        {
            return _cacheProvider.Get<NoticeStudentsOfClassDayBucket>(tenantId, new NoticeStudentsOfClassDayBucket().GetKeyFormat(tenantId, classOt));
        }

        public void SetNoticeStudentsOfClassDayBucket(int tenantId, DateTime classOt)
        {
            var bucket = new NoticeStudentsOfClassDayBucket()
            {
                ClassOt = classOt
            };
            var key = bucket.GetKeyFormat(tenantId, classOt);
            _cacheProvider.Set(tenantId, key, bucket, bucket.TimeOut);
        }
    }
}
