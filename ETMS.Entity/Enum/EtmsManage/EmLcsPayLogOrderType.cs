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
        /// 转课
        /// </summary>
        public const byte TransferCourse = 2;

        /// <summary>
        /// 账户充值
        /// </summary>
        public const int StudentAccountRecharge = 3;

        /// <summary>
        /// 小禾招生
        /// </summary>
        public const int Activity = 4;
        public static string GetPayLogOrderTypeDesc(int t)
        {
            switch (t)
            {
                case 0:
                    return "报名/续费";
                case 2:
                    return "转课";
                case 3:
                    return "账户充值";
                case Activity:
                    return "小禾招生";
            }
            return string.Empty;
        }
    }
}
