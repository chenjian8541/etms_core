using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class SuitBucket : ICacheDataContract
    {
        public EtSuit Suit { get; set; }

        public List<EtSuitDetail> SuitDetails { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SuitBucket_{parms[0]}_{parms[1]}";
        }
    }
}
