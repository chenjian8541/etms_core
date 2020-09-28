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

        public static string GetClassRecordAbsenceHandleStatusDesc(byte t)
        {
            return t == EmClassRecordAbsenceHandleStatus.Unprocessed ? "未处理" : "已补";
        }
    }
}
