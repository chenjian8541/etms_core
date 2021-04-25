using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 订单折扣类别
    /// </summary>
    public struct EmDiscountType
    {
        /// <summary>
        /// 无折扣
        /// </summary>
        public const byte Nothing = 0;

        /// <summary>
        /// 直减
        /// </summary>
        public const byte DeductionMoney = 1;

        /// <summary>
        /// 打折
        /// </summary>
        public const byte Discount = 2;
    }
}
