using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmActiveHomeworkDetailAnswerStatus
    {
        /// <summary>
        /// 未提交
        /// </summary>
        public const byte Unanswered = 0;

        /// <summary>
        /// 已完成
        /// </summary>
        public const byte Answered = 1;

        public static string ActiveHomeworkDetailAnswerStatusDesc(byte t)
        {
            if (t == Unanswered)
            {
                return "未提交";
            }
            return "已完成";
        }
    }
}
