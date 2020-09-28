using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmExchangeType
    {
        /// <summary>
        /// 前台
        /// </summary>
        public const byte Reception = 0;

        /// <summary>
        /// 微信
        /// </summary>
        public const byte WeChat = 1;

        public static string GetExchangeType(byte b)
        {
            if (b == WeChat)
            {
                return "前台";
            }
            else
            {
                return "微信";
            }
        }
    }
}
