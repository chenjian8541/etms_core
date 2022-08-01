using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmAchievementStatus
    {
        /// <summary>
        /// 保存
        /// </summary>
        public const byte Save = 0;

        /// <summary>
        /// 发布
        /// </summary>
        public const byte Publish = 1;

        public static string GetAchievementStatusDesc(int t)
        {
            return t == Save ? "未发布" : "已发布";
        }
    }
}
