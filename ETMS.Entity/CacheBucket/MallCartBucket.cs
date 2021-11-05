using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class MallCartBucket : ICacheDataContract
    {
        public TimeSpan TimeOut { get; } = TimeSpan.FromMinutes(10);

        public MallCartView MallCartView { get; set; }

        public string GetKeyFormat(params object[] parms)
        {
            return $"MallCartBucket_{parms[0]}_{parms[1]}";
        }
    }
}
