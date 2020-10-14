using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class WxGzhAccessTokenBucket : ICacheDataContract
    {
        public string AppId { get; set; }

        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public DateTime ExTime { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromHours(BucketTimeOutConfig.WxAccessTokenBucket);

        /// <summary>
        /// appid
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"WxGzhAccessTokenBucket_{parms[0]}";
        }
    }
}

