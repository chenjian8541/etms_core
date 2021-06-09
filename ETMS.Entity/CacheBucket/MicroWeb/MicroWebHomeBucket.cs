using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.MicroWeb
{
    public class MicroWebHomeBucket : ICacheDataContract
    {
        public MicroWebHomeView MicroWebHomeView { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// TenantId
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"MicroWebHomeBucket_{parms[0]}";
        }
    }
}
