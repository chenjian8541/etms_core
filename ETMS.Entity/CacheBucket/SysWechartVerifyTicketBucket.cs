using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Entity.CacheBucket
{
    public class SysWechartVerifyTicketBucket : ICacheDataContract
    {
        public SysWechartVerifyTicket SysWechartVerifyTicket { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TenantDataTimeOutDay);

        /// <summary>
        /// ComponentAppId
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"SysWechartVerifyTicketBucket_{parms[0]}";
        }
    }
}
