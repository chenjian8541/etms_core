using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmMallOrderStatus
    {
        /// <summary>
        /// 正常(已支付)
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 未支付(待支付)
        /// </summary>
        public const byte Unpaid = 1;

        /// <summary>
        /// 待补交
        /// </summary>
        public const byte MakeUpMoney = 2;

        /// <summary>
        /// 已作废
        /// </summary>
        public const byte Repeal = 3;

        public static string GetMallOrderStatusDesc(byte t)
        {
            switch (t)
            {
                case Normal:
                    return "正常";
                case Unpaid:
                    return "待支付";
                case MakeUpMoney:
                    return "待补交";
                case Repeal:
                    return "已作废";
            }
            return string.Empty;
        }
    }
}
