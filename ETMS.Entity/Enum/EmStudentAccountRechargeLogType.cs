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
        public const int OrderReturn = 3;

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
                case OrderReturn:
                    return "退款至账户";
            }
            return string.Empty;
        }

        public static byte GetValueChangeType(int type)
        {
            switch (type)
            {
                case Recharge:
                    return EmValueChangeType.Add;
                case Refund:
                    return EmValueChangeType.Deduction;
                case Pay:
                    return EmValueChangeType.Deduction;
                case OrderReturn:
                    return EmValueChangeType.Add;
            }
            return EmValueChangeType.Add;
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
                case OrderReturn:
                    return $"+￥{value.ToString("F2")}";
            }
            return string.Empty;
        }
    }
}
