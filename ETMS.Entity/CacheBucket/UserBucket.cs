using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    /// <summary>
    /// 用户缓存  TenantId+UserId
    /// </summary>
    public class UserBucket : ICacheDataContract
    {
        public EtUser User { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"UserBucket_{parms[0]}_{parms[1]}";
        }
    }
}
