using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class StudentBucket : ICacheDataContract
    {
        public EtStudent Student { get; set; }

        public List<EtStudentExtendInfo> StudentExtendInfos { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"StudentBucket_{parms[0]}_{parms[1]}";
        }
    }
}
