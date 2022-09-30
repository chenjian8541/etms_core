using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct SuixingPayExtendType
    {
        /// <summary>
        /// 扫码支付
        /// </summary>
        public const byte BarcodePay = 1;

        /// <summary>
        /// 微信商城
        /// </summary>
        public const byte WeChatMall = 2;
    }
}
