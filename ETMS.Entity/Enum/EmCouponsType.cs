using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 优惠券类型
    /// </summary>
    public struct EmCouponsType
    {
        /// <summary>
        /// 代金券
        /// </summary>
        public const byte Cash = 0;

        /// <summary>
        /// 折扣券
        /// </summary>
        public const byte Discount = 1;

        /// <summary>
        /// 课时券
        /// </summary>
        public const byte ClassTimes = 2;

        public static string GetCouponsTypeDesc(byte type)
        {
            switch (type)
            {
                case EmCouponsType.Cash:
                    return "代金券";
                case EmCouponsType.Discount:
                    return "折扣券";
                default:
                    return "课时券";
            }
        }
    }
}
