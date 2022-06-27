using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class ActivityRouteSimpleBucket: ICacheDataContract
    {
        public EtActivityRoute ActivityRoute { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(1);

        public string GetKeyFormat(params object[] parms)
        {
            return $"ActivityRouteSimpleBucket_{parms[0]}_{parms[1]}";
        }
    }
}
