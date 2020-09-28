using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 上课记录状态
    /// </summary>
    public struct EmClassRecordStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 已撤销
        /// </summary>
        public const byte Revoked = 1;

        public static string GetClassRecordStatusDesc(byte t)
        {
            return t == EmClassRecordStatus.Normal ? "正常" : "已撤销";
        }
    }
}
