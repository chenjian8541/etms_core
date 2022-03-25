using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.Alien
{
    public class MgUserLoginFailedBucket : ICacheDataContract
    {
        public string TenantCode { get; set; }

        public string Phone { get; set; }

        public int FailedCount { get; set; }

        public DateTime ExpireAtTime { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromMinutes(30);

        public string GetKeyFormat(params object[] parms)
        {
            return $"MgUserLoginFailedBucket_{parms[0]}_{parms[1]}";
        }
    }
}

