using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysAIFaceBiduAccountBucket : ICacheDataContract
    {
        public SysAIFaceBiduAccount SysAIFaceBiduAccount { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TenantDataTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysAIFaceBiduAccountBucket_{parms[0]}";
        }
    }
}
