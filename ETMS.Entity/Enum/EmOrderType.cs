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
        /// 退课程
        /// </summary>
        public const byte ReturnCourse = 1;

        /// <summary>
        /// 退物品
        /// </summary>
        public const byte ReturnGoods = 2;

        /// <summary>
        /// 退费用
        /// </summary>
        public const byte ReturnCost = 3;
    }
}
