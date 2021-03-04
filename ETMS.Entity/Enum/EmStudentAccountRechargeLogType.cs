using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmStudentAccountRechargeLogType
    {
        /// <summary>
        /// 账户充值
        /// </summary>
        public const int Recharge = 0;

        /// <summary>
        /// 账户退款
        /// </summary>
        public const int Refund = 1;

        /// <summary>
        /// 账户支出
        /// </summary>
        public const int Pay = 2;

        /// <summary>
        /// 退款至账户
        /// </summary>
        public const int OrderRetuen = 3;

        public static string GetStudentAccountRechargeLogTypeDesc(int type)
        {
            switch (type)
            {
                case Recharge:
                    return "账户充值";
                case Refund:
                    return "账户退款";
                case Pay:
                    return "账户支出";
                case OrderRetuen:
                    return "退款至账户";
            }
            return string.Empty;
        }

        public static string GetValueDesc(decimal value, int type)
        {
            if (value == 0)
            {
                return "-";
            }
            switch (type)
            {
                case Recharge:
                    return $"+￥{value.ToString("F2")}";
                case Refund:
                    return $"-￥{value.ToString("F2")}";
                case Pay:
                    return $"-￥{value.ToString("F2")}";
                case OrderRetuen:
                    return $"+￥{value.ToString("F2")}";
            }
            return string.Empty;
        }
    }
}
