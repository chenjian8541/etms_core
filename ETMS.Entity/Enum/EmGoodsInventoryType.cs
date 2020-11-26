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

        /// <summary>
        /// 初始化库存
        /// </summary>
        public const byte AddGoodsInInventory = 4;

        /// <summary>
        /// 编辑库存
        /// </summary>
        public const byte EditGoodsInInventoryAdd = 5;

        /// <summary>
        /// 编辑库存
        /// </summary>
        public const byte EditGoodsInInventoryDe = 6;

        public static int GetGoodsInventoryChangeQuantity(int type, int quantity)
        {
            switch (type)
            {
                case InInventory:
                    return quantity;
                case Sale:
                    return -quantity;
                case ReturnGoods:
                    return quantity;
                case OrderStudentEnrolmentRepeal:
                    return quantity;
                case AddGoodsInInventory:
                    return quantity;
                case EditGoodsInInventoryAdd:
                    return quantity;
                case EditGoodsInInventoryDe:
                    return -quantity;
            }
            return quantity;
        }

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
                case AddGoodsInInventory:
                    return "初始化库存";
                case EditGoodsInInventoryAdd:
                case EditGoodsInInventoryDe:
                    return "编辑库存";
            }
            return string.Empty;
        }
    }
}
