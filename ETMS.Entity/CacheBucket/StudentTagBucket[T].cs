using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class StudentTagBucket<T> : ICacheDataContract, ICacheSimpleDataContract<T>
        where T : EtStudentTag
    {
        public TimeSpan TimeOut { get; set; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);
        public List<T> Entitys { get; set; }

        public string GetKeyFormat(params object[] parms)
        {
            return $"StudentTagBucket_{parms[0]}";
        }
    }
}
