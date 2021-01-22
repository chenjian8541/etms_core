using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmSysExplainType
    {
        /// <summary>
        /// 帮助中心
        /// </summary>
        public const int HelpCenter = 0;

        /// <summary>
        /// 升级公告
        /// </summary>
        public const int UpgradeMsg = 1;

        public static string GetSysExplainTypeDesc(int t)
        {
            if (t == HelpCenter)
            {
                return "帮助中心";
            }
            return "升级公告";
        }
    }
}
