using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.TempShortTime
{
    public class NoticeStudentsOfClassDayBucket : ICacheDataContract
    {
        public DateTime ClassOt { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.NoticeStudentsOfClassBeforeDay);

        public string GetKeyFormat(params object[] parms)
        {
            var ot = Convert.ToDateTime(parms[1]);
            var otKey = ot.ToString("yyyyMMdd");
            return $"NoticeStudentsOfClassDayBucket_{parms[0]}_{otKey}";
        }
    }
}
