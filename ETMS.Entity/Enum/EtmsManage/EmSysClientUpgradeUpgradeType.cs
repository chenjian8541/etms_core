using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysClientUpgradeUpgradeType
    {
        /// <summary>
        /// 非强制
        /// </summary>
        public const byte NotForcibly = 0;

        /// <summary>
        /// 强制更新
        /// </summary>
        public const byte Forcibly = 1;

        public static string GetSysClientUpgradeUpgradeTypeDesc(byte type)
        {
            if (type == NotForcibly)
            {
                return "非强制";
            }
            return "强制更新";
        }
    }
}
