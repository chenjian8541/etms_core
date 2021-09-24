using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmLcsPayLogStatus
    {
        /// <summary>
        /// 未支付
        /// </summary>
        public const int Unpaid = 0;

        /// <summary>
        /// 支付中
        /// </summary>
        public const int Paying = 1;

        /// <summary>
        /// 支付成功
        /// </summary>
        public const int PaySuccess = 2;

        /// <summary>
        /// 支付失败
        /// </summary>
        public const int PayFail = 3;
    }
}
