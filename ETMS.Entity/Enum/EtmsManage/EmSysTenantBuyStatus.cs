using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysTenantBuyStatus
    {
        /// <summary>
        /// 测试账号
        /// </summary>
        public const byte Test = 0;

        /// <summary>
        /// 正式账号
        /// </summary>
        public const byte Official = 1;

        public static string GetSysTenantBuyStatusDesc(byte t)
        {
            if (t == Test)
            {
                return "测试账号";
            }
            return "正式账号";
        }
    }
}
