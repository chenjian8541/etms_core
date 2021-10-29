using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmMallOrderStatus
    {
        /// <summary>
        /// 正常(已支付)
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 未支付(待支付)
        /// </summary>
        public const byte Unpaid = 1;

        /// <summary>
        /// 待补交
        /// </summary>
        public const byte MakeUpMoney = 2;

        /// <summary>
        /// 已退款
        /// </summary>
        public const byte Refund = 3;
    }
}
