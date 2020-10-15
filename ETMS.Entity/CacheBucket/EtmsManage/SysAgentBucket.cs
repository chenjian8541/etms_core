using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysAgentBucket : ICacheDataContract
    {
        public SysAgent SysAgent { get; set; }

        public List<SysAgentEtmsAccount> SysAgentEtmsAccounts { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysAgentBucket_{parms[0]}";
        }
    }
}
