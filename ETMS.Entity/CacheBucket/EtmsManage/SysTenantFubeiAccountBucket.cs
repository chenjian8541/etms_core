using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysTenantFubeiAccountBucket : ICacheDataContract
    {
        public SysTenantFubeiAccount TenantFubeiAccount { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TenantDataTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysTenantFubeiAccountBucket_{parms[0]}";
        }
    }
}
