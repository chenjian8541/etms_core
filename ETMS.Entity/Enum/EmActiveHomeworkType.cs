using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmActiveHomeworkType
    {
        /// <summary>
        /// 单次作业
        /// </summary>
        public const byte SingleWork = 0;

        /// <summary>
        /// 连续作业
        /// </summary>
        public const byte ContinuousWork = 1;

        public static string GetActiveHomeworkTypeDesc(byte t)
        {
            if (t == SingleWork)
            {
                return "单次作业";
            }
            return "连续作业";
        }
    }
}
