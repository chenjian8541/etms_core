using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmOrderBuyType
    {
        /// <summary>
        /// 新报
        /// </summary>
        public const byte New = 0;

        /// <summary>
        /// 续报
        /// </summary>
        public const byte Renew = 1;

        /// <summary>
        /// 扩科
        /// </summary>
        public const byte Expand = 2;

        public static string GetOrderBuyTypeDesc(byte t)
        {
            switch (t)
            {
                case New:
                    return "新报";
                case Renew:
                    return "续报";
                case Expand:
                    return "扩科";
            }
            return string.Empty;
        }
    }
}
