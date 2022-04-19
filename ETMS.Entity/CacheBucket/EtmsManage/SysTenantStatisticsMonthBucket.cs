using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysTenantStatisticsMonthBucket : ICacheDataContract
    {
        public SysTenantStatisticsMonth SysTenantStatisticsMonth { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysTenantStatisticsMonthBucket_{parms[0]}";
        }
    }
}
