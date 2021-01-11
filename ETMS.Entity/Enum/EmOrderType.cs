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
    }
}
