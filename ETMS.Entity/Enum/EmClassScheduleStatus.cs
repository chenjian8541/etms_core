using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 班级排课状态
    /// </summary>
    public struct EmClassScheduleStatus
    {
        /// <summary>
        /// 未排课
        /// </summary>
        public const byte Unscheduled = 0;

        /// <summary>
        /// 已排课
        /// </summary>
        public const byte Scheduled = 1;

        public static string GetClassScheduleStatusDesc(byte status)
        {
            switch (status)
            {
                case EmClassScheduleStatus.Unscheduled:
                    return "未排课";
                default:
                    return "已排课";
            }
        }
    }
}
