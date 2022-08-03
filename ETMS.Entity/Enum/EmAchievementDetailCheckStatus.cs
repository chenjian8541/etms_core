using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmAchievementDetailCheckStatus
    {
        /// <summary>
        /// 缺勤
        /// </summary>
        public const byte NotArrived = 0;

        /// <summary>
        /// 参加考试
        /// </summary>
        public const byte Join = 1;

        public static string GetAchievementDetailCheckStatusDesc(byte t)
        {
            return t == NotArrived ? "缺勤" : "参加考试";
        }
    }
}
