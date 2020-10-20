using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmActiveHomeworkDetailReadStatus
    {
        /// <summary>
        /// 未读
        /// </summary>
        public const byte No = 0;

        /// <summary>
        /// 已读
        /// </summary>
        public const byte Yes = 1;

        public static string GetActiveHomeworkDetailReadStatusDesc(byte t)
        {
            if (t == EmActiveHomeworkDetailReadStatus.No)
            {
                return "未读";
            }
            return "已读";
        }
    }
}
