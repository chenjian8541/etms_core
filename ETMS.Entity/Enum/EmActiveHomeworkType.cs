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

        public static string GetActiveHomeworkTypeDesc(byte t)
        {
            return "单次作业";
        }
    }
}
