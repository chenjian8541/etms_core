using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess
{
    public class UserChangePwdSmsCodeDAL : IUserChangePwdSmsCodeDAL
    {
        /// <summary>
        /// 缓存访问提供器
        /// </summary>
        private ICacheProvider _cacheProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheProvider"></param>
        public UserChangePwdSmsCodeDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public UserChangePwdSmsCodeBucket GetUserChangePwdSmsCode(int tenantId, long userId)
        {
            return _cacheProvider.Get<UserChangePwdSmsCodeBucket>(tenantId, new UserChangePwdSmsCodeBucket().GetKeyFormat(tenantId, userId));
        }

        public UserChangePwdSmsCodeBucket AddUserChangePwdSmsCode(int tenantId, long userId, string smsCode)
        {
            var key = new UserChangePwdSmsCodeBucket().GetKeyFormat(tenantId, userId);
            var record = _cacheProvider.Get<UserChangePwdSmsCodeBucket>(tenantId, key);
            if (record == null)
            {
                record = new UserChangePwdSmsCodeBucket()
                {
                    TenantId = tenantId,
                    UserId = userId,
                    SmsCode = smsCode
                };
            }
            record.SmsCode = smsCode;
            record.ExpireAtTime = DateTime.Now.AddMinutes(SystemConfig.UserSafetyConfig.UserChangePwdSmsCodeTimeOut);
            _cacheProvider.Set(tenantId, key, record, record.TimeOut);
            return record;
        }

        public void RemoveUserChangePwdSmsCode(int tenantId, long userId)
        {
            var key = new UserChangePwdSmsCodeBucket().GetKeyFormat(tenantId, userId);
            _cacheProvider.Remove(tenantId, key);
        }
    }
}
