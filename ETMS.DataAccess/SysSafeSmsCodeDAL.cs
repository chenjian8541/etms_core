using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess
{
    public class SysSafeSmsCodeDAL : ISysSafeSmsCodeDAL
    {
        /// <summary>
        /// 缓存访问提供器
        /// </summary>
        private ICacheProvider _cacheProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheProvider"></param>
        public SysSafeSmsCodeDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public SysSafeSmsCodeBucket GetSysSafeSmsCode(int tenantId)
        {
            return _cacheProvider.Get<SysSafeSmsCodeBucket>(tenantId, new SysSafeSmsCodeBucket().GetKeyFormat(tenantId));
        }

        public SysSafeSmsCodeBucket AddSysSafeSmsCode(int tenantId, string smsCode)
        {
            var key = new SysSafeSmsCodeBucket().GetKeyFormat(tenantId);
            var record = _cacheProvider.Get<SysSafeSmsCodeBucket>(tenantId, key);
            if (record == null)
            {
                record = new SysSafeSmsCodeBucket()
                {
                    SmsCode = smsCode
                };
            }
            record.SmsCode = smsCode;
            record.ExpireAtTime = DateTime.Now.AddMinutes(SystemConfig.SysSafetyConfig.SysSafeSmsCodeTimeOut);
            _cacheProvider.Set(tenantId, key, record, record.TimeOut);
            return record;
        }

        public void RemoveSysSafeSmsCode(int tenantId)
        {
            var key = new SysSafeSmsCodeBucket().GetKeyFormat(tenantId);
            _cacheProvider.Remove(tenantId, key);
        }
    }
}
