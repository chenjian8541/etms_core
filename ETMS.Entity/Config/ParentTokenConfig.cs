using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    /// <summary>
    /// 家长端存储的token信息
    /// </summary>
    public class ParentTokenConfig
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public string ExTimestamp { get; set; }
    }
}
