using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmStudentCheckOnLogStatus
    {
        /// <summary>
        /// 正常(未记上课)
        /// </summary>
        public const byte NormalNotClass = 0;

        /// <summary>
        /// 正常(记上课)
        /// </summary>
        public const byte NormalAttendClass = 1;

        /// <summary>
        /// 已点名
        /// </summary>
        public const byte BeRollcall = 2;

        /// <summary>
        /// 撤销记上课
        /// </summary>
        public const byte Revoke = 3;

        public static string GetStatusDesc(byte t)
        {
            switch (t)
            {
                case NormalNotClass:
                    return "考勤";
                case NormalAttendClass:
                    return "考勤(记上课)";
                case BeRollcall:
                    return "考勤(已点名)";
                case Revoke:
                    return "考勤(撤销记上课)";
            }
            return string.Empty;
        }
    }
}
