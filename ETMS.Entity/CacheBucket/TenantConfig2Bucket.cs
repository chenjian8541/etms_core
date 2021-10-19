using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket
{
    public class TenantConfig2Bucket : ICacheDataContract
    {
        public TenantConfig2 TenantConfig2 { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"TenantConfig2Bucket_{parms[0]}";
        }
    }
}

