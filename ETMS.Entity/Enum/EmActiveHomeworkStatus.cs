using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmActiveHomeworkStatus
    {
        /// <summary>
        /// 待提交作业
        /// </summary>
        public const byte Undone = 0;

        /// <summary>
        /// 已完成作业
        /// </summary>
        public const byte Finish = 1;
    }
}
