using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员试听来源
    /// </summary>
    public struct EmTryCalssSourceType
    {
        /// <summary>
        /// 微信
        /// </summary>
        public const byte WeChat = 0;

        public static string GetTryCalssSourceTypeDesc(byte b)
        {
            if (b == WeChat)
            {
                return "微信";
            }
            return string.Empty;
        }
    }
}
