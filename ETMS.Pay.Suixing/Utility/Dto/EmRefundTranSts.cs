using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto
{
    public struct EmRefundTranSts
    {
        /// <summary>
        /// 退款成功
        /// </summary>
        public const string REFUNDSUC = "REFUNDSUC";

        /// <summary>
        /// 退款失败
        /// </summary>
        public const string REFUNDFAIL = "REFUNDFAIL";

        /// <summary>
        /// 退款中
        /// </summary>
        public const string REFUNDING = "REFUNDING";
    }
}
