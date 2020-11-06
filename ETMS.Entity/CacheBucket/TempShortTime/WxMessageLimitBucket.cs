using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.TempShortTime
{
    public class WxMessageLimitBucket : ICacheDataContract
    {
        public int TotalCount { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TempTwoDays);

        /// <summary>
        /// 机构+时间
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            var ot = Convert.ToDateTime(parms[1]);
            var otKey = ot.ToString("yyyyMMdd");
            return $"WxMessageLimitBucket_{parms[0]}_{otKey}";
        }
    }
}
