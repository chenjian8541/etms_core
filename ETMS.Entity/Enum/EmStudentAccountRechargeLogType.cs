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

        /// <summary>
        /// 推荐有奖(注册)
        /// </summary>
        public const int RecommendStudentRegistered = 4;

        /// <summary>
        /// 推荐有奖(报名/续费)
        /// </summary>
        public const int RecommendStudentBuy = 5;

        /// <summary>
        /// 多支付(报名/续费)
        /// </summary>
        public const int StudentContractsOverpayment = 6;

        /// <summary>
        /// 多支付(导入学员课程)
        /// </summary>
        public const int StudentImportOverpayment = 7;

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
                case RecommendStudentRegistered:
                    return "推荐有奖(注册)";
                case RecommendStudentBuy:
                    return "推荐有奖(报名/续费)";
                case StudentContractsOverpayment:
                    return "多支付(报名/续费)";
                case StudentImportOverpayment:
                    return "多支付(导入学员课程)";
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
                case RecommendStudentRegistered:
                    return EmValueChangeType.Add;
                case RecommendStudentBuy:
                    return EmValueChangeType.Add;
                case StudentContractsOverpayment:
                    return EmValueChangeType.Add;
                case StudentImportOverpayment:
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
                case RecommendStudentRegistered:
                    return $"+￥{value.ToString("F2")}";
                case RecommendStudentBuy:
                    return $"+￥{value.ToString("F2")}";
                case StudentContractsOverpayment:
                    return $"+￥{value.ToString("F2")}";
                case StudentImportOverpayment:
                    return $"+￥{value.ToString("F2")}";
            }
            return string.Empty;
        }
    }
}
