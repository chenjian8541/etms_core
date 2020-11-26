using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess
{
    public class SysDataClearSmsCodeDAL : ISysDataClearSmsCodeDAL
    {
        /// <summary>
        /// 缓存访问提供器
        /// </summary>
        private ICacheProvider _cacheProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheProvider"></param>
        public SysDataClearSmsCodeDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public SysDataClearSmsCodeBucket GetSysDataClearSmsCode(int tenantId)
        {
            return _cacheProvider.Get<SysDataClearSmsCodeBucket>(tenantId, new SysDataClearSmsCodeBucket().GetKeyFormat(tenantId));
        }

        public SysDataClearSmsCodeBucket AddSysDataClearSmsCode(int tenantId, string smsCode)
        {
            var key = new SysDataClearSmsCodeBucket().GetKeyFormat(tenantId);
            var record = _cacheProvider.Get<SysDataClearSmsCodeBucket>(tenantId, key);
            if (record == null)
            {
                record = new SysDataClearSmsCodeBucket()
                {
                    SmsCode = smsCode
                };
            }
            record.SmsCode = smsCode;
            record.ExpireAtTime = DateTime.Now.AddMinutes(SystemConfig.SysSafetyConfig.SysDataClearSmsCodeTimeOut);
            _cacheProvider.Set(tenantId, key, record, record.TimeOut);
            return record;
        }

        public void RemoveSysDataClearSmsCode(int tenantId)
        {
            var key = new SysDataClearSmsCodeBucket().GetKeyFormat(tenantId);
            _cacheProvider.Remove(tenantId, key);
        }
    }
}
