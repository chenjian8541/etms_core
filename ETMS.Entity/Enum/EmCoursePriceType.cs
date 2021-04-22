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
        /// 按天
        /// </summary>
        public const byte Day = 2;

        /// <summary>
        /// 多种方式
        /// </summary>
        public const byte MultipleWays = 9;

        public static string GetCoursePriceTypeDesc(byte type)
        {
            switch (type)
            {
                case EmCoursePriceType.ClassTimes:
                    return "按课时";
                case EmCoursePriceType.Month:
                    return "按月";
                case EmCoursePriceType.Day:
                    return "按天";
                default:
                    return string.Empty;
            }
        }

        public static string GetGetCourseUnitDesc(byte type)
        {
            switch (type)
            {
                case EmCoursePriceType.ClassTimes:
                    return "课时";
                case EmCoursePriceType.Month:
                    return "月";
                case EmCoursePriceType.Day:
                    return "天";
                default:
                    return string.Empty;
            }
        }

        public static string GetCoursePriceTypeDesc2(byte type, string priceTypeDesc)
        {
            if (!string.IsNullOrEmpty(priceTypeDesc))
            {
                return priceTypeDesc;
            }
            if (type == 2) //处理之前的数据  2为按课时&按月
            {
                return "按课时&按月";
            }
            return GetCoursePriceTypeDesc(type);
        }
    }
}
