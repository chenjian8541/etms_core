using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    /// <summary>
    /// 机构
    /// </summary>
    public class SysTenantConfigBucket : ICacheDataContract
    {
        /// <summary>
        /// 机构信息
        /// </summary>
        public ViewTenantConfig TenantConfigs { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TenantDataTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysTenantConfigBucket_{parms[0]}";
        }
    }
}
