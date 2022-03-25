using ETMS.Entity.Config;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.Alien
{
    public class MgUserBucket : ICacheDataContract
    {
        public MgUser MyMgUser { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// HeadId+Id
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"MgUserBucket_{parms[0]}_{parms[1]}";
        }
    }
}
