using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.DataAccess
{
    public class SysPhoneSmsCodeDAL : ISysPhoneSmsCodeDAL
    {
        /// <summary>
        /// 缓存访问提供器
        /// </summary>
        private ICacheProvider _cacheProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheProvider"></param>
        public SysPhoneSmsCodeDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public SysPhoneSmsCodeBucket GetSysPhoneSmsCode(string phone)
        {
            return _cacheProvider.Get<SysPhoneSmsCodeBucket>(0, new SysPhoneSmsCodeBucket().GetKeyFormat(phone));
        }

        public SysPhoneSmsCodeBucket AddSysPhoneSmsCode(string phone, string smsCode)
        {
            var key = new SysPhoneSmsCodeBucket().GetKeyFormat(phone);
            var record = _cacheProvider.Get<SysPhoneSmsCodeBucket>(0, key);
            if (record == null)
            {
                record = new SysPhoneSmsCodeBucket()
                {
                    SmsCode = smsCode
                };
            }
            record.SmsCode = smsCode;
            record.ExpireAtTime = DateTime.Now.AddMinutes(SystemConfig.SysSafetyConfig.SysSafeSmsCodeTimeOut);
            _cacheProvider.Set(0, key, record, record.TimeOut);
            return record;
        }

        public void RemoveSysPhoneSmsCode(string phone)
        {
            var key = new SysSafeSmsCodeBucket().GetKeyFormat(phone);
            _cacheProvider.Remove(0, key);
        }
    }
}
