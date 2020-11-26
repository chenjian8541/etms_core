using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class ClearDataBucket : ICacheDataContract
    {
        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ClearDataSaveDays);

        public int TotalCount { get; set; }

        public string GetKeyFormat(params object[] parms)
        {
            var timeDesc = Convert.ToDateTime(parms[1]).ToString("yyyyMM");
            return $"ClearDataBucket_{parms[0]}_{timeDesc}";
        }
    }
}
