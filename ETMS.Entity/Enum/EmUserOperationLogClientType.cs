using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmUserOperationLogClientType
    {
        /// <summary>
        /// PC
        /// </summary>
        public const int PC = 0;

        /// <summary>
        /// 微信
        /// </summary>
        public const int WeChat = 1;

        public static string GetClientTypeDesc(int type)
        {
            if (type == PC)
            {
                return "PC端";
            }
            return "微信公众号";
        }
    }
}
