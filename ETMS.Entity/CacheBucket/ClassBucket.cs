﻿using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class ClassBucket : ICacheDataContract
    {
        public EtClass EtClass { get; set; }

        public List<EtClassStudent> EtClassStudents { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"ClassBucket_{parms[0]}_{parms[1]}";
        }
    }
}
