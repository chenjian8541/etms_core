using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class UserChangePwdSmsCodeBucket : ICacheDataContract
    {
        public int TenantId { get; set; }

        public long UserId { get; set; }

        public string SmsCode { get; set; }

        public DateTime ExpireAtTime { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromMinutes(30);

        public string GetKeyFormat(params object[] parms)
        {
            return $"UserChangePwdSmsCodeBucket_{parms[0]}_{parms[1]}";
        }
    }
}
