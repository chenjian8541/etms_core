using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 系统全局配置
    /// </summary>
    public struct EmSysAppsettingsType
    {
        /// <summary>
        /// 机构默认授权信息(绑定的小禾帮微信公众号)
        /// </summary>
        public const byte TenantDefaultWechartAuth = 0;

        /// <summary>
        /// 腾讯云账户信息
        /// </summary>
        public const byte TencentCloudAccount = 1;

        /// <summary>
        /// 系统配置信息
        /// </summary>
        public const byte EtmsGlobalConfig = 2;
    }
}
