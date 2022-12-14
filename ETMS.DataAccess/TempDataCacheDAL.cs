using ETMS.Entity.CacheBucket;
using ETMS.Entity.CacheBucket.MicroWeb;
using ETMS.Entity.CacheBucket.Other;
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

        public UserLoginOnlineBucket GetUserLoginOnlineBucket(int tenantId, long userId, int loginClientType)
        {
            return _cacheProvider.Get<UserLoginOnlineBucket>(0, new UserLoginOnlineBucket().GetKeyFormat(tenantId, userId, loginClientType));
        }

        public void SetUserLoginOnlineBucket(int tenantId, long userId, string loginTime, int loginClientType)
        {
            var bucket = new UserLoginOnlineBucket()
            {
                LoginTime = loginTime,
                TenantId = tenantId,
                UserId = userId,
                LoginClientType = loginClientType
            };
            var key = bucket.GetKeyFormat(tenantId, userId, loginClientType);
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

        public StudentCheckLastTimeBucket GetStudentCheckLastTimeBucket(int tenantId, long studentId)
        {
            return _cacheProvider.Get<StudentCheckLastTimeBucket>(0, new StudentCheckLastTimeBucket().GetKeyFormat(tenantId, studentId));
        }

        public void SetStudentCheckLastTimeBucket(int tenantId, long studentId, DateTime lastCheckTime)
        {
            var bucket = new StudentCheckLastTimeBucket()
            {
                StudentCheckLastTime = lastCheckTime
            };
            var key = bucket.GetKeyFormat(tenantId, studentId);
            _cacheProvider.Set(0, key, bucket, bucket.TimeOut);
        }

        public BaiduCloudAccessTokenBucket GetBaiduCloudAccessTokenBucket(string appid)
        {
            return _cacheProvider.Get<BaiduCloudAccessTokenBucket>(0, new BaiduCloudAccessTokenBucket().GetKeyFormat(appid));
        }

        public void SetBaiduCloudAccessTokenBucket(BaiduCloudAccessTokenBucket bucket)
        {
            var key = bucket.GetKeyFormat(bucket.Appid);
            _cacheProvider.Set(0, key, bucket, bucket.TimeOut);
        }

        public MicroWebHomeBucket GetMicroWebHomeBucket(int tenantId)
        {
            return _cacheProvider.Get<MicroWebHomeBucket>(tenantId, new MicroWebHomeBucket().GetKeyFormat(tenantId));
        }

        public void SetMicroWebHomeBucket(int tenantId, MicroWebHomeBucket bucket)
        {
            var key = bucket.GetKeyFormat(tenantId);
            _cacheProvider.Set(tenantId, key, bucket, bucket.TimeOut);
        }

        public void RemoveMicroWebHomeBucket(int tenantId)
        {
            _cacheProvider.Remove(tenantId, new MicroWebHomeBucket().GetKeyFormat(tenantId));
        }

        public PhoneVerificationCodeBucket GetPhoneVerificationCodeBucket(string phone)
        {
            return _cacheProvider.Get<PhoneVerificationCodeBucket>(0, new PhoneVerificationCodeBucket().GetKeyFormat(phone));
        }

        public void SetPhoneVerificationCodeBucket(string phone, string verificationCode)
        {
            var bucket = new PhoneVerificationCodeBucket()
            {
                Phone = phone,
                VerificationCode = verificationCode
            };
            var key = bucket.GetKeyFormat(phone);
            _cacheProvider.Set(0, key, bucket, bucket.TimeOut);
        }

        public void RemovePhoneVerificationCodeBucket(string phone)
        {
            _cacheProvider.Remove(0, new PhoneVerificationCodeBucket().GetKeyFormat(phone));
        }

        public void SetUserTenantEntrancePCBucket(int tenantId, UserTenantEntrancePCBucket bucket)
        {
            var key = bucket.GetKeyFormat(tenantId, bucket.MyUserLoginOutput.UId);
            _cacheProvider.Set(tenantId, key, bucket, bucket.TimeOut);
        }

        public void RemoveUserTenantEntrancePCBucket(int tenantId, long userId)
        {
            _cacheProvider.Remove(tenantId, new UserTenantEntrancePCBucket().GetKeyFormat(tenantId, userId));
        }

        public UserTenantEntrancePCBucket GetUserTenantEntrancePCBucket(int tenantId, long userId)
        {
            return _cacheProvider.Get<UserTenantEntrancePCBucket>(tenantId, new UserTenantEntrancePCBucket().GetKeyFormat(tenantId, userId));
        }
    }
}
