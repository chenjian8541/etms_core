using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 课程单位
    /// </summary>
    public struct EmCourseUnit
    {
        /// <summary>
        /// 课时
        /// </summary>
        public const byte ClassTimes = 0;

        /// <summary>
        /// 天
        /// </summary>
        public const byte Day = 1;

        /// <summary>
        /// 月
        /// </summary>
        public const byte Month = 2;

        public static string GetCourseUnitDesc(byte b)
        {
            switch (b)
            {
                case ClassTimes:
                    return "课时";
                case Day:
                    return "天";
                case Month:
                    return "月";
            }
            return string.Empty;
        }
    }
}
