using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmSmsLogStatus
    {
        /// <summary>
        /// 发送中
        /// </summary>
        public const byte IsSending = 0;

        /// <summary>
        /// 已完成
        /// </summary>
        public const byte Finish = 1;
    }
}
