using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员意向级别
    /// </summary>
    public struct EmStudentIntentionLevel
    {
        /// <summary>
        /// 低
        /// </summary>
        public const int Low = 0;

        /// <summary>
        /// 中
        /// </summary>
        public const int Middle = 1;

        /// <summary>
        /// 高
        /// </summary>
        public const int High = 2;

        public static string GetIntentionLevelDesc(int level)
        {
            switch (level)
            {
                case EmStudentIntentionLevel.Low:
                    return "低";
                case EmStudentIntentionLevel.Middle:
                    return "中";
                default:
                    return "高";
            }
        }
    }
}
