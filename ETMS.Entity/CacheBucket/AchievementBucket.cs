using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class AchievementBucket : ICacheDataContract
    {
        public EtAchievement Achievement { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);
        
        /// <summary>
        /// 机构ID  id
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"AchievementBucket_{parms[0]}_{parms[1]}";
        }
    }
}
