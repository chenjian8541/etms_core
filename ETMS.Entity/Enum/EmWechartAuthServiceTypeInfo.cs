using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 公众号类型
    /// </summary>
    public struct EmWechartAuthServiceTypeInfo
    {
        /// <summary>
        /// 订阅号
        /// </summary>
        public const string ServiceType0 = "0";

        /// <summary>
        /// 由历史老帐号升级后的订阅号
        /// </summary>
        public const string ServiceType1 = "1";

        /// <summary>
        /// 服务号
        /// </summary>
        public const string ServiceType2 = "2";
    }
}
