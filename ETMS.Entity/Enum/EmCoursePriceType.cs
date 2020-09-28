using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 课程收费类型
    /// </summary>
    public struct EmCoursePriceType
    {
        /// <summary>
        /// 按课时
        /// </summary>
        public const byte ClassTimes = 0;

        /// <summary>
        /// 按月
        /// </summary>
        public const byte Month = 1;

        /// <summary>
        /// 课时和月
        /// </summary>
        public const byte ClassTimesAndMonth = 2;

        public static string GetCoursePriceTypeDesc(byte type)
        {
            switch (type)
            {
                case EmCoursePriceType.ClassTimes:
                    return "按课时";
                case EmCoursePriceType.Month:
                    return "按月";
                default:
                    return "按课时&按月";
            }
        }
    }
}
