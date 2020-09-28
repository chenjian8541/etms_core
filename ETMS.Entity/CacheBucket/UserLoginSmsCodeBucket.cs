using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class UserLoginSmsCodeBucket : ICacheDataContract
    {
        public string TenantCode { get; set; }

        public string Phone { get; set; }

        public string SmsCode { get; set; }

        public DateTime ExpireAtTime { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromHours(1);

        public string GetKeyFormat(params object[] parms)
        {
            return $"UserLoginSmsCodeBucket_{parms[0]}_{parms[1]}";
        }
    }
}
