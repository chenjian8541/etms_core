using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class ActivityVisitorBucket : ICacheDataContract
    {
        public EtActivityVisitor ActivityVisitor { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromHours(2);

        /// <summary>
        /// 机构+ActivityId+MiniPgmUserId
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"ActivityVisitorBucket_{parms[0]}_{parms[1]}_{parms[2]}";
        }
    }
}
