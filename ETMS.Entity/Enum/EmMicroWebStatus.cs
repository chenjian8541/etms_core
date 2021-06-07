using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmMicroWebStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        public const byte Enable = 0;

        /// <summary>
        /// 禁用
        /// </summary>
        public const byte Disable = 1;

        public static string GetMicroWebStatusDesc(byte t)
        {
            return t == Enable ? "启用" : "禁用";
        }
    }
}
