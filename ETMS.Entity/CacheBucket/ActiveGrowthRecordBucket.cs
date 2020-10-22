using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class ActiveGrowthRecordBucket : ICacheDataContract
    {
        public EtActiveGrowthRecord ActiveGrowthRecord { get; set; }

        public List<EtActiveGrowthRecordDetailComment> Comments { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"ActiveGrowthRecordBucket_{parms[0]}_{parms[1]}";
        }
    }
}
