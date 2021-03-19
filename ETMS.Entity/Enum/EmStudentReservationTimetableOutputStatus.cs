using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmStudentReservationTimetableOutputStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 已结束
        /// </summary>
        public const byte Over = 1;

        /// <summary>
        /// 已预约
        /// </summary>
        public const byte IsReservationed = 2;

        public static string GetStudentReservationTimetableOutputStatusDesc(byte t)
        {
            switch (t)
            {
                case Normal:
                    return "正常";
                case Over:
                    return "已结束";
                case IsReservationed:
                    return "已预约";
            }
            return string.Empty;
        }
    }
}
