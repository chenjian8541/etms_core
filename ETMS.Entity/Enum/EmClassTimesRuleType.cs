using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 班级排课规则类型
    /// </summary>
    public struct EmClassTimesRuleType
    {
        /// <summary>
        /// 不循环
        /// </summary>
        public const byte UnLoop = 0;

        /// <summary>
        /// 循环
        /// </summary>
        public const byte Loop = 1;
    }
}
