using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;


namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysWechatMiniPgmUserBucket : ICacheDataContract
    {
        public SysWechatMiniPgmUser WechatMiniPgmUser { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.TempOneDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"SysWechatMiniPgmUserBucket_{WechatMiniPgmUser.OpenId}";
        }
    }
}
