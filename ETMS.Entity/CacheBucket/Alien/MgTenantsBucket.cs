using ETMS.Entity.Config;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.Alien
{
    public class MgTenantsBucket : ICacheDataContract
    {
        public List<MgTenants> TenantsList { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"MgTenantsBucket_{parms[0]}";
        }
    }
}
