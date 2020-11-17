using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysWechartAuthTemplateMsgBucket : ICacheDataContract
    {
        public List<SysWechartAuthTemplateMsg> SysWechartAuthTemplateMsgs { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TenantDataTimeOutDay);

        /// <summary>
        /// AuthorizerAppid
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"SysWechartAuthTemplateMsgBucket_{parms[0]}";
        }
    }
}
