using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;


namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysTenantCloudStorageBucket : ICacheDataContract
    {
        public List<SysTenantCloudStorage> StorageLogs { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// TenantId
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"SysTenantCloudStorageBucket_{parms[0]}";
        }
    }
}
