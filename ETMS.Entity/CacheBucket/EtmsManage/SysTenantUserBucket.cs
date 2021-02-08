using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysTenantUserBucket : ICacheDataContract
    {
        public List<SysTenantUser> SysTenantUsers { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromHours(BucketTimeOutConfig.SysTenantPeopleOutHour);

        /// <summary>
        /// phone
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"SysTenantUserBucket_{parms[0]}";
        }
    }
}
