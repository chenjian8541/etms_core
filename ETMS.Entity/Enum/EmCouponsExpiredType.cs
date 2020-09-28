using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 优惠券过期类型
    /// </summary>
    public struct EmCouponsExpiredType
    {
        /// <summary>
        /// 不过期
        /// </summary>
        public const int Unexpected = 0;

        /// <summary>
        /// 固定时间
        /// </summary>
        public const int FixedTime = 1;

        /// <summary>
        /// 领取后
        /// </summary>
        public const int AfterGet = 2;
    }
}
