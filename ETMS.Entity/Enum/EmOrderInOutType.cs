using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmOrderInOutType
    {
        /// <summary>
        /// 收款
        /// </summary>
        public const byte In = 0;

        /// <summary>
        /// 退款
        /// </summary>
        public const byte Out = 1;

        public static string GetTotalPointsDesc(int totalPoints, byte orderInOutType)
        {
            if (orderInOutType == In)
            {
                return $"+{totalPoints}";
            }
            return $"-{totalPoints}";
        }

        public static string GetOrderInOutTypeDesc(byte orderInOutType)
        {
            return orderInOutType == In ? "收款" : "退款";
        }
    }
}
