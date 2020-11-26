using ETMS.Entity.CacheBucket;
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

        public WxGzhAccessTokenBucket GetWxGzhAccessTokenBucket(string appid)
        {
            return _cacheProvider.Get<WxGzhAccessTokenBucket>(0, new WxGzhAccessTokenBucket().GetKeyFormat(appid));
        }

        public void SetWxGzhAccessTokenBucket(WxGzhAccessTokenBucket bucket, string appid)
        {
            var key = bucket.GetKeyFormat(appid);
            _cacheProvider.Set(0, key, bucket, bucket.TimeOut);
        }

        public UserLoginOnlineBucket GetUserLoginOnlineBucket(int tenantId, long userId)
        {
            return _cacheProvider.Get<UserLoginOnlineBucket>(0, new UserLoginOnlineBucket().GetKeyFormat(tenantId, userId));
        }

        public void SetUserLoginOnlineBucket(int tenantId, long userId, string loginTime)
        {
            var bucket = new UserLoginOnlineBucket()
            {
                LoginTime = loginTime,
                TenantId = tenantId,
                UserId = userId
            };
            var key = bucket.GetKeyFormat(tenantId, userId);
            _cacheProvider.Set(0, key, bucket, bucket.TimeOut);
        }

        public WxMessageLimitBucket GetWxMessageLimitBucket(int tenantId, DateTime time)
        {
            return _cacheProvider.Get<WxMessageLimitBucket>(0, new WxMessageLimitBucket().GetKeyFormat(tenantId, time));
        }

        public void SetWxMessageLimitBucket(int tenantId, DateTime time, int totalCount)
        {
            var bucket = new WxMessageLimitBucket()
            {
                TotalCount = totalCount
            };
            var key = bucket.GetKeyFormat(tenantId, time);
            _cacheProvider.Set(0, key, bucket, bucket.TimeOut);
        }

        public ClearDataBucket GetClearDataBucket(int tenantId, DateTime time)
        {
            return _cacheProvider.Get<ClearDataBucket>(0, new ClearDataBucket().GetKeyFormat(tenantId, time));
        }

        public void SetClearDataBucket(int tenantId, DateTime time, int totalCount)
        {
            var bucket = new ClearDataBucket()
            {
                TotalCount = totalCount
            };
            var key = bucket.GetKeyFormat(tenantId, time);
            _cacheProvider.Set(0, key, bucket, bucket.TimeOut);
        }
    }
}
