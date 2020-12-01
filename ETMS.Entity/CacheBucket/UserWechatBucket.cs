using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;

namespace ETMS.Entity.CacheBucket
{
    public class UserWechatBucket : ICacheDataContract
    {
        public EtUserWechat UserWechat { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// 机构ID 用户ID
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"UserWechatBucket_{parms[0]}_{parms[1]}";
        }
    }
}
