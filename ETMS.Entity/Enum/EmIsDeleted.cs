using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 是否删除
    /// </summary>
    public struct EmIsDeleted
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 已删除
        /// </summary>
        public const byte Deleted = 1;
    }
}
