using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 订单产品类型
    /// </summary>
    public struct EmProductType
    {
        /// <summary>
        /// 课程
        /// </summary>
        public const byte Course = 0;

        /// <summary>
        /// 物品
        /// </summary>
        public const byte Goods = 1;

        /// <summary>
        /// 费用
        /// </summary>
        public const byte Cost = 2;

        /// <summary>
        /// 套餐
        /// </summary>
        public const byte Suit = 3;

        public static string GetProductType(byte b)
        {
            switch (b)
            {
                case Course:
                    return "课程";
                case Goods:
                    return "物品";
                case Cost:
                    return "费用";
                case Suit:
                    return "套餐";
            }
            return string.Empty;
        }
    }
}
