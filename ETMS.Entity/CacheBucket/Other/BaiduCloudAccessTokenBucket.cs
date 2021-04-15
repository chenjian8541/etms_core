using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.Other
{
    public class BaiduCloudAccessTokenBucket : ICacheDataContract
    {
        public string Appid { get; set; }

        public string AccessToken { get; set; }

        public DateTime ExTime { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(30);

        public string GetKeyFormat(params object[] parms)
        {
            return $"BaiduCloudAccessTokenBucket_{parms[0]}";
        }
    }
}
