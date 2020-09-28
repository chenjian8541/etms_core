using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 物品库存变动类型
    /// </summary>
    public struct EmGoodsInventoryType
    {
        /// <summary>
        /// 入库
        /// </summary>
        public const byte InInventory = 0;

        /// <summary>
        /// 销售
        /// </summary>
        public const byte Sale = 1;

        /// <summary>
        /// 退物品
        /// </summary>
        public const byte ReturnGoods = 2;

        /// <summary>
        /// 订单作废
        /// </summary>
        public const byte OrderStudentEnrolmentRepeal = 3;

        public static string GetGoodsInventoryTypeDesc(int type)
        {
            switch (type)
            {
                case InInventory:
                    return "入库";
                case Sale:
                    return "销售";
                case ReturnGoods:
                    return "退物品";
                case OrderStudentEnrolmentRepeal:
                    return "订单作废";
            }
            return string.Empty;
        }
    }
}
