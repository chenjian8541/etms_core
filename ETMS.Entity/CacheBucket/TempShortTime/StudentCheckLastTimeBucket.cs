using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.TempShortTime
{
    public class StudentCheckLastTimeBucket : ICacheDataContract
    {
        public DateTime StudentCheckLastTime { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TempTwoDays);

        /// <summary>
        /// 机构+学员ID
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"StudentCheckLastTimeBucket_{parms[0]}_{parms[1]}";
        }
    }
}
