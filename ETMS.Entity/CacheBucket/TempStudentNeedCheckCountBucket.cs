using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class TempStudentNeedCheckCountBucket : ICacheDataContract
    {
        public int NeedCheckInCount { get; set; }

        public int NeedCheckOutCount { get; set; }

        public int NeedAttendClassCount { get; set; }

        public int NeedCheckCount { get; set; }

        public int FinishCheckInCount { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TempTwoDays);

        public string GetKeyFormat(params object[] parms)
        {
            var ot = Convert.ToDateTime(parms[1]);
            var otKey = ot.ToString("yyyyMMdd");
            return $"TempStudentNeedCheckCountBucket_{parms[0]}_{otKey}";
        }
    }
}
