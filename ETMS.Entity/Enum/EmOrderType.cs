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
            }
            return string.Empty;
        }

        public static string GetTotalPointsDesc(int totalPoints)
        {
            if (totalPoints > 0)
            {
                return $"+{totalPoints}";
            }
            return $"-{totalPoints}";
        }
    }
}
