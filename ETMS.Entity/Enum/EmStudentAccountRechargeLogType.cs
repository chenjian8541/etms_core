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
    }
}
