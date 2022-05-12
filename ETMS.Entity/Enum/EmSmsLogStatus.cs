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

        /// <summary>
        /// 失败
        /// </summary>
        public const byte Fail = 3;

        public static string GetSmsLogStatusDesc(byte t)
        {
            if (t == Finish)
            {
                return "已完成";
            }
            return t == Fail ? "发送失败" : "发送中";
        }
    }
}
