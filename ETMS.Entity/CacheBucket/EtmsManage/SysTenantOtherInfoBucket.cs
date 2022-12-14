using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysTenantOtherInfoBucket : ICacheDataContract
    {
        public SysTenantOtherInfo SysTenantOtherInfo { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TenantDataTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysTenantOtherInfoBucket_{parms[0]}";
        }
    }
}
