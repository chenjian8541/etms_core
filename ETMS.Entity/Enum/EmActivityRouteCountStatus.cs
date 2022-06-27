using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmActivityRouteCountStatus
    {
        /// <summary>
        /// 未成团
        /// </summary>
        public const byte None = 0;

        /// <summary>
        /// 已成团
        /// </summary>
        public const byte CompleteItem = 1;

        /// <summary>
        /// 已满团
        /// </summary>
        public const byte CompleteFull = 2;

        public static string ActivityRouteCountStatusTag(byte t)
        {
            switch (t)
            {
                case None:
                    return "开团成功";
                case CompleteItem:
                    return "已成团";
                case CompleteFull:
                    return "已满团";
            }
            return string.Empty;
        }
    }
}
