using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysSmsTemplateBucket : ICacheDataContract
    {
        public List<SysSmsTemplate> SysSmsTemplates { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysSmsTemplateBucket_{parms[0]}";
        }
    }
}
