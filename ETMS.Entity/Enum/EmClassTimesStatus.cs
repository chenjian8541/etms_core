using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 班级排课记录状态
    /// </summary>
    public struct EmClassTimesStatus
    {
        /// <summary>
        /// 未点名
        /// </summary>
        public const byte UnRollcall = 0;

        /// <summary>
        /// 已点名
        /// </summary>
        public const byte BeRollcall = 1;
    }
}
