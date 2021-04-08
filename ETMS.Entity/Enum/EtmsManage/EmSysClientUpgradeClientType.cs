using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysClientUpgradeClientType
    {
        /// <summary>
        /// 安卓端
        /// </summary>
        public const int Android = 0;

        public static string GetSysClientUpgradeClientTypeDesc(int type)
        {
            return "安卓端";
        }
    }
}
