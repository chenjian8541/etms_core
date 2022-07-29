using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmAchievementSourceType
    {
        /// <summary>
        /// 机构内
        /// </summary>
        public const int TenantIn = 0;

        /// <summary>
        /// 机构外
        /// </summary>
        public const int TenantOut = 1;

        public static string GetAchievementSourceTypeDesc(int t)
        {
            return t == TenantIn ? "机构内" : "机构外";
        }
    }
}
