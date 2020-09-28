using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class CourseBucket : ICacheDataContract
    {
        public EtCourse Course { get; set; }

        public List<EtCoursePriceRule> CoursePriceRules { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"CourseBucket_{parms[0]}_{parms[1]}";
        }
    }
}

