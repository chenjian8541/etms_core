using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class UserLoginOnlineBucket : ICacheDataContract
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 登录的时间戳
        /// </summary>
        public string LoginTime { get; set; }

        /// <summary>
        /// timeout时间
        /// </summary>
        public TimeSpan TimeOut { get; set; } = TimeSpan.FromDays(BucketTimeOutConfig.UserLoginOnlineDay);

        /// <summary>
        /// 客户端类型  <see cref="EmUserOperationLogClientType"/>
        /// </summary>
        public int LoginClientType { get; set; }

        /// <summary>
        /// 机构+用户ID+客户端类型
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"UserLoginOnlineBucket_{parms[0]}_{parms[1]}_{parms[2]}";
        }
    }
}
