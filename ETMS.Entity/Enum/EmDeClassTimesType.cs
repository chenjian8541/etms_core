using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 扣课时规则
    /// </summary>
    public struct EmDeClassTimesType
    {
        /// <summary>
        /// 按课时
        /// </summary>
        public const byte ClassTimes = 0;

        /// <summary>
        /// 按月份(按天消耗)
        /// </summary>
        public const byte Day = 1;

        /// <summary>
        /// 未扣
        /// </summary>
        public const byte NotDe = 2;

        public static string GetDeClassTimesTypeDesc(byte t)
        {
            switch (t)
            {
                case EmDeClassTimesType.ClassTimes:
                    return "按课时";
                case EmDeClassTimesType.Day:
                    return "按天消耗";
                default:
                    return "未扣课时";
            }
        }
    }
}
