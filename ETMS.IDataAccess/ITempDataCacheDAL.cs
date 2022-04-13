using ETMS.Entity.CacheBucket;
using ETMS.Entity.CacheBucket.MicroWeb;
using ETMS.Entity.CacheBucket.Other;
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

        UserLoginOnlineBucket GetUserLoginOnlineBucket(int tenantId, long userId, int loginClientType);

        void SetUserLoginOnlineBucket(int tenantId, long userId, string loginTime, int loginClientType);

        WxMessageLimitBucket GetWxMessageLimitBucket(int tenantId, DateTime time);

        void SetWxMessageLimitBucket(int tenantId, DateTime time, int totalCount);

        ClearDataBucket GetClearDataBucket(int tenantId, DateTime time);

        void SetClearDataBucket(int tenantId, DateTime time, int totalCount);

        StudentCheckLastTimeBucket GetStudentCheckLastTimeBucket(int tenantId, long studentId);

        void SetStudentCheckLastTimeBucket(int tenantId, long studentId, DateTime lastCheckTime);

        BaiduCloudAccessTokenBucket GetBaiduCloudAccessTokenBucket(string appid);

        void SetBaiduCloudAccessTokenBucket(BaiduCloudAccessTokenBucket bucket);

        MicroWebHomeBucket GetMicroWebHomeBucket(int tenantId);

        void SetMicroWebHomeBucket(int tenantId, MicroWebHomeBucket bucket);

        void RemoveMicroWebHomeBucket(int tenantId);

        PhoneVerificationCodeBucket GetPhoneVerificationCodeBucket(string phone);

        void SetPhoneVerificationCodeBucket(string phone, string verificationCode);

        void RemovePhoneVerificationCodeBucket(string phone);

        void SetUserTenantEntrancePCBucket(int tenantId, UserTenantEntrancePCBucket bucket);

        void RemoveUserTenantEntrancePCBucket(int tenantId, long userId);

        UserTenantEntrancePCBucket GetUserTenantEntrancePCBucket(int tenantId, long userId);
    }
}
