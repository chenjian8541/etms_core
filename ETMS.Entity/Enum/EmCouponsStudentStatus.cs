using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员优惠券状态
    /// </summary>
    public struct EmCouponsStudentStatus
    {
        /// <summary>
        /// 未使用
        /// </summary>
        public const byte Unused = 0;

        /// <summary>
        /// 已核销
        /// </summary>
        public const byte Used = 1;

        /// <summary>
        /// 已过期
        /// </summary>
        public const byte Expired = 2;
    }
}
