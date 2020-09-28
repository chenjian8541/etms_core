using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public struct EmOrderStatus
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
        /// 作废,无效
        /// </summary>
        public const byte Repeal = 3;

        public static string GetOrderStatus(byte status)
        {
            switch (status)
            {
                case EmOrderStatus.Normal:
                    return "已支付";
                case EmOrderStatus.Unpaid:
                    return "待支付";
                case EmOrderStatus.MakeUpMoney:
                    return "待补交";
                case EmOrderStatus.Repeal:
                    return "已作废";
            }
            return string.Empty;
        }
    }
}
