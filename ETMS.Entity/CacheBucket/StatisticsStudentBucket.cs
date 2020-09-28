using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class StatisticsStudentBucket : ICacheDataContract
    {
        public List<EtStatisticsStudent> StatisticsStudents { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"StatisticsStudentBucket_{parms[0]}";
        }
    }
}
