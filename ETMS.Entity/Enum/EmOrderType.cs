using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public struct EmOrderType
    {
        /// <summary>
        /// 学员报名
        /// </summary>
        public const byte StudentEnrolment = 0;

        /// <summary>
        /// 退单
        /// </summary>
        public const byte ReturnOrder = 1;

        /// <summary>
        /// 转课
        /// </summary>
        public const byte TransferCourse = 2;

        /// <summary>
        /// 账户充值
        /// </summary>
        public const byte StudentAccountRecharge = 3;

        /// <summary>
        /// 账户退款
        /// </summary>
        public const byte StudentAccountRefund = 4;

        public static bool IsBuyOrder(int orderType)
        {
            switch (orderType)
            {
                case StudentEnrolment:
                    return true;
                case ReturnOrder:
                    return true;
                case TransferCourse:
                    return true;
                case StudentAccountRecharge:
                    return false;
                case StudentAccountRefund:
                    return false;
            }
            return false;
        }

        public static string GetOrderTypeDesc(int orderType)
        {
            switch (orderType)
            {
                case StudentEnrolment:
                    return "报名/续费";
                case ReturnOrder:
                    return "销售退单";
                case TransferCourse:
                    return "转课";
                case StudentAccountRecharge:
                    return "账户充值";
                case StudentAccountRefund:
                    return "账户退款";
            }
            return string.Empty;
        }
    }
}
