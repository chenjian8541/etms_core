using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess
{
    public class ParentLoginSmsCodeDAL : IParentLoginSmsCodeDAL
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
        public ParentLoginSmsCodeDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public ParentLoginSmsCodeBucket GetParentLoginSmsCode(string code, string phone)
        {
            return _cacheProvider.Get<ParentLoginSmsCodeBucket>(_tenantId, new ParentLoginSmsCodeBucket().GetKeyFormat(code, phone));
        }

        public ParentLoginSmsCodeBucket AddParentLoginSmsCode(string code, string phone, string smsCode)
        {
            var key = new ParentLoginSmsCodeBucket().GetKeyFormat(code, phone);
            var record = _cacheProvider.Get<ParentLoginSmsCodeBucket>(_tenantId, key);
            if (record == null)
            {
                record = new ParentLoginSmsCodeBucket()
                {
                    Phone = phone,
                    TenantCode = code,
                    SmsCode = smsCode
                };
            }
            record.SmsCode = smsCode;
            record.ExpireAtTime = DateTime.Now.AddMinutes(SystemConfig.ParentAccessConfig.LoginSmsCodeTimeOut);
            _cacheProvider.Set(_tenantId, key, record, record.TimeOut);
            return record;
        }

        public void RemoveParentLoginSmsCode(string code, string phone)
        {
            var key = new ParentLoginSmsCodeBucket().GetKeyFormat(code, phone);
            _cacheProvider.Remove(_tenantId, key);
        }
    }
}
