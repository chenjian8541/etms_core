using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class TempStudentClassNoticeBucket : ICacheDataContract
    {
        public List<EtTempStudentClassNotice> TempStudentClassNotices { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TempStudentClassNotice);

        public string GetKeyFormat(params object[] parms)
        {
            var ot = Convert.ToDateTime(parms[1]);
            var otKey = ot.ToString("yyyyMMdd");
            return $"TempStudentClassNoticeBucket_{parms[0]}_{otKey}";
        }
    }
}
