using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Entity.CacheBucket
{
    public class ActivityRouteBucket : ICacheDataContract
    {
        public EtActivityRoute ActivityRoute { get; set; }

        public List<EtActivityRouteItem> ActivityRouteItems { get; set; }

        public List<EtActivityHaggleLog> ActivityHaggleLogs { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构+Id
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"ActivityRouteBucket_{parms[0]}_{parms[1]}";
        }
    }
}
