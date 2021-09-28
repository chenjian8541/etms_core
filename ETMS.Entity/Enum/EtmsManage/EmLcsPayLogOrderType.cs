using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmLcsPayLogOrderType
    {
        /// <summary>
        /// 报名/续费
        /// </summary>
        public const int StudentEnrolment = 0;

        /// <summary>
        /// 账户充值
        /// </summary>
        public const int StudentAccountRecharge = 3;

        /// <summary>
        /// 在线购课
        /// </summary>
        public const int OnlineBuyCourse = 4;

        public static string GetPayLogOrderTypeDesc(int t)
        {
            switch (t)
            {
                case 0:
                    return "报名/续费";
                case 3:
                    return "账户充值";
                case 4:
                    return "在线购课";
            }
            return string.Empty;
        }
    }
}
