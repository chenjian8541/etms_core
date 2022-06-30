using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmActivityRoutePayStatus
    {
        /// <summary>
        /// 未支付
        /// </summary>
        public const int Unpaid = 0;

        /// <summary>
        /// 已支付
        /// </summary>
        public const int Paid = 1;

        /// <summary>
        /// 已退款
        /// </summary>
        public const int Refunded = 2;

        public static string GetActivityRoutePayStatusDesc(int t)
        {
            switch (t)
            {
                case Unpaid:
                    return "未支付";
                case Paid:
                    return "已支付";
                case Refunded:
                    return "已退款";
            }
            return string.Empty;
        }
    }
}
