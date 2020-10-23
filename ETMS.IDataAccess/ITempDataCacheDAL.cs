using ETMS.Entity.CacheBucket;
using ETMS.Entity.CacheBucket.TempShortTime;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITempDataCacheDAL
    {
        NoticeStudentsOfClassDayBucket GetNoticeStudentsOfClassDayBucket(int tenantId, DateTime classOt);

        void SetNoticeStudentsOfClassDayBucket(int tenantId, DateTime classOt);

        WxGzhAccessTokenBucket GetWxGzhAccessTokenBucket(string appid);

        void SetWxGzhAccessTokenBucket(WxGzhAccessTokenBucket bucket, string appid);

        UserLoginOnlineBucket GetUserLoginOnlineBucket(int tenantId, long userId);

        void SetUserLoginOnlineBucket(int tenantId, long userId, string loginTime);
    }
}
