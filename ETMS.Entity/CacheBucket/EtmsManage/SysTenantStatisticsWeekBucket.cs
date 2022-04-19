using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;


namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysTenantStatisticsWeekBucket : ICacheDataContract
    {
        public SysTenantStatisticsWeek SysTenantStatisticsWeek { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysTenantStatisticsWeekBucket_{parms[0]}";
        }
    }
}
