using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmCoursePriceRuleExpiredType
    {
        /// <summary>
        /// 天
        /// </summary>
        public const byte Day = 1;

        /// <summary>
        /// 月
        /// </summary>
        public const byte Month = 2;

        /// <summary>
        /// 年
        /// </summary>
        public const byte Year = 3;

        public static string GetCoursePriceRuleExpiredTypeDesc(byte t)
        {
            switch (t)
            {
                case Day:
                    return "天";
                case Month:
                    return "个月";
                case Year:
                    return "年";
            }
            return string.Empty;
        }
    }
}
