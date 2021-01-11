using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 订单操作日志类型
    /// </summary>
    public struct EmOrderOperationLogType
    {
        /// <summary>
        /// 补交费用
        /// </summary>
        public const byte CollectMoney = 0;

        /// <summary>
        /// 修改业绩归属人
        /// </summary>
        public const byte ModifyCommissionUser = 1;

        /// <summary>
        /// 编辑备注
        /// </summary>
        public const byte ModifyRemark = 2;

        /// <summary>
        /// 作废,无效
        /// </summary>
        public const byte Repeal = 3;

        /// <summary>
        /// 销售退单
        /// </summary>
        public const byte OrderReturn = 4;

        public static string GetOrderOperationLogTypeDesc(int t)
        {
            switch (t)
            {
                case CollectMoney:
                    return "补交费用";
                case ModifyCommissionUser:
                    return "修改业绩归属人";
                case ModifyRemark:
                    return "编辑备注";
                case Repeal:
                    return "订单作废";
                case OrderReturn:
                    return "销售退单";
            }
            return string.Empty;
        }
    }
}
