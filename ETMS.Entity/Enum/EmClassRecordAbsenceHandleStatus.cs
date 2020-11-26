using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 缺勤处理状态
    /// </summary>
    public struct EmClassRecordAbsenceHandleStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        public const byte Unprocessed = 0;

        /// <summary>
        /// 标记已补(已完成)
        /// </summary>
        public const byte MarkFinish = 1;

        /// <summary>
        /// 插班补课
        /// </summary>
        public const byte MakeupClassTimes = 2;

        public static string GetClassRecordAbsenceHandleStatusDesc(byte t)
        {
            switch (t)
            {
                case Unprocessed:
                    return "未处理";
                case MarkFinish:
                    return "已补";
                case MakeupClassTimes:
                    return "插班补课";
            }
            return string.Empty;
        }
    }
}
