using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    public interface IRedisToken
    {
        /// <summary>
        /// 商户ID
        /// </summary>
        int TenantId { get; set; }

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        TimeSpan TimeOut { get; }

        string GetKey();
    }
}
