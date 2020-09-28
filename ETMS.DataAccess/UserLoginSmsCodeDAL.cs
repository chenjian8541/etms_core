using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess
{
    public class UserLoginSmsCodeDAL : IUserLoginSmsCodeDAL
    {
        /// <summary>
        /// 缓存访问提供器
        /// </summary>
        private ICacheProvider _cacheProvider;

        protected int _tenantId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheProvider"></param>
        public UserLoginSmsCodeDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public UserLoginSmsCodeBucket GetUserLoginSmsCode(string code, string phone)
        {
            return _cacheProvider.Get<UserLoginSmsCodeBucket>(_tenantId, new UserLoginSmsCodeBucket().GetKeyFormat(code, phone));
        }

        public UserLoginSmsCodeBucket AddUserLoginSmsCode(string code, string phone, string smsCode)
        {
            var key = new UserLoginSmsCodeBucket().GetKeyFormat(code, phone);
            var record = _cacheProvider.Get<UserLoginSmsCodeBucket>(_tenantId, key);
            if (record == null)
            {
                record = new UserLoginSmsCodeBucket()
                {
                    Phone = phone,
                    TenantCode = code,
                    SmsCode = smsCode
                };
            }
            record.SmsCode = smsCode;
            record.ExpireAtTime = DateTime.Now.AddMinutes(SystemConfig.UserLoginConfig.LoginSmsCodeTimeOut);
            _cacheProvider.Set(_tenantId, key, record, record.TimeOut);
            return record;
        }

        public void RemoveUserLoginSmsCode(string code, string phone)
        {
            var key = new UserLoginSmsCodeBucket().GetKeyFormat(code, phone);
            _cacheProvider.Remove(_tenantId, key);
        }
    }
}
