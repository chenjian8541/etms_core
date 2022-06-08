using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysTenantSuixingAccountBucket : ICacheDataContract
    {
        public SysTenantSuixingAccount TenantSuixingAccount { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TenantDataTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysTenantSuixingAccountBucket_{parms[0]}";
        }
    }
}
