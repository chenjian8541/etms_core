using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.ShareTemplate
{
    public class ShareTemplateIdBucket : ICacheDataContract
    {
        public EtShareTemplate MyShareTemplate { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构ID_记录ID
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"ShareTemplateIdBucket_{parms[0]}_{parms[1]}";
        }
    }
}
