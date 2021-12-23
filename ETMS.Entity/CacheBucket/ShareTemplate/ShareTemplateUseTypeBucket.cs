using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.ShareTemplate
{
    public class ShareTemplateUseTypeBucket : ICacheDataContract
    {
        /// <summary>
        /// 分享链接
        /// </summary>
        public EtShareTemplate MyShareTemplateLink { get; set; }

        /// <summary>
        /// 海报
        /// </summary>
        public EtShareTemplate MyShareTemplatePoster { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构ID_适用类型
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"ShareTemplateUseTypeBucket_{parms[0]}_{parms[1]}";
        }
    }
}
