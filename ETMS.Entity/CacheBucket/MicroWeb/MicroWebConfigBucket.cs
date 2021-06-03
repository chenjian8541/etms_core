using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.MicroWeb
{
    public class MicroWebConfigBucket : ICacheDataContract
    {
        public EtMicroWebConfig MicroWebConfig { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// TenantId+Type
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"MicroWebConfigBucket_{parms[0]}_{parms[1]}";
        }
    }
}
