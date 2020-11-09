using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class WxMessageParentReadBucket : ICacheDataContract
    {
        public int UnreadCount { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 机构+手机号码
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"WxMessageParentReadBucket_{parms[0]}_{parms[1]}";
        }
    }
}
