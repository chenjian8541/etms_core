using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员到课状态
    /// </summary>
    public struct EmClassStudentCheckStatus
    {
        /// <summary>
        /// 到课
        /// </summary>
        public const byte Arrived = 0;

        /// <summary>
        /// 迟到
        /// </summary>
        public const byte BeLate = 1;

        /// <summary>
        /// 请假
        /// </summary>
        public const byte Leave = 2;

        /// <summary>
        /// 未到
        /// </summary>
        public const byte NotArrived = 3;

        public static string GetClassStudentCheckStatus(byte status)
        {
            switch (status)
            {
                case EmClassStudentCheckStatus.Arrived:
                    return "到课";
                case EmClassStudentCheckStatus.BeLate:
                    return "迟到";
                case EmClassStudentCheckStatus.Leave:
                    return "请假";
                case EmClassStudentCheckStatus.NotArrived:
                    return "未到";
            }
            return string.Empty;
        }
    }
}
