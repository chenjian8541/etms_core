using ETMS.Entity.Config;
using ETMS.Entity.Config.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysVersionMenuConfigBucket : ICacheDataContract
    {
        public List<MenuConfig> MenuConfigs { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TenantDataTimeOutDay);

        /// <summary>
        /// 系统版本ID
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"SysVersionMenuConfigBucket_{parms[0]}";
        }
    }
}

