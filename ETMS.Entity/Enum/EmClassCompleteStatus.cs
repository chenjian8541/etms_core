using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 班级结业状态
    /// </summary>
    public struct EmClassCompleteStatus
    {
        /// <summary>
        /// 未结课
        /// </summary>
        public const byte UnComplete = 0;

        /// <summary>
        /// 已结课
        /// </summary>
        public const byte Completed = 1;

        public static string GetClassCompleteStatusDesc(byte status)
        {
            switch (status)
            {
                case EmClassCompleteStatus.UnComplete:
                    return "未结业";
                default:
                    return "已结业";
            }
        }
    }
}
