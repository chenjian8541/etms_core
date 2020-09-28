using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class ParentLoginSmsCodeBucket : ICacheDataContract
    {
        public string TenantCode { get; set; }

        public string Phone { get; set; }

        public string SmsCode { get; set; }

        public DateTime ExpireAtTime { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromHours(1);

        public string GetKeyFormat(params object[] parms)
        {
            return $"ParentLoginSmsCodeBucket_{parms[0]}_{parms[1]}";
        }
    }
}
