using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class SysWechartAuthorizerTokenBucket : ICacheDataContract
    {
        public SysWechartAuthorizerToken SysWechartAuthorizerToken { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TenantDataTimeOutDay);

        /// <summary>
        /// AuthorizerAppid
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"SysWechartAuthorizerTokenBucket_{parms[0]}";
        }
    }
}
