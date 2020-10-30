using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmSysTenantWechartAuthAuthorizeState
    {
        /// <summary>
        /// 取消授权
        /// </summary>
        public const byte Unauthorized = 0;

        /// <summary>
        /// 授权成功
        /// </summary>
        public const byte Authorized = 1;
    }
}
