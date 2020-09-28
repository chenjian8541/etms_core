using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class CostBucket : ICacheDataContract
    {
        public EtCost Cost { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"CostBucket_{parms[0]}_{parms[1]}";
        }
    }
}
