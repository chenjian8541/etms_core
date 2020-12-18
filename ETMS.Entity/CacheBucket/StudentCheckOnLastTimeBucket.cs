using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class StudentCheckOnLastTimeBucket : ICacheDataContract
    {
        public EtStudentCheckOnLog StudentCheckOnLog { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"StudentCheckOnLastTimeBucket_{parms[0]}_{parms[1]}";
        }
    }
}
