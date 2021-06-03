using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.MicroWeb
{
    public class MicroWebColumnArticleBucket : ICacheDataContract
    {
        public EtMicroWebColumnArticle MicroWebColumnArticle { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TempOneDay);

        /// <summary>
        /// TenantId+Id
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"MicroWebColumnArticleBucket_{parms[0]}_{parms[1]}";
        }
    }
}