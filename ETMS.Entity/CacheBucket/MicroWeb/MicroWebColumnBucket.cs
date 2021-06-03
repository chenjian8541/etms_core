using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.MicroWeb
{
    public class MicroWebColumnBucket : ICacheDataContract
    {
        public List<EtMicroWebColumn> MicroWebColumns { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// TenantId
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"MicroWebColumnBucket_{parms[0]}";
        }
    }
}
