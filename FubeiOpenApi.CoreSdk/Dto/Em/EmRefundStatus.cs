using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Fubei.OpenApi.Sdk.Dto.Em
{
    public struct EmRefundStatus
    {
        /// <summary>
        /// 退款中
        /// </summary>
        public const string Refunding = "REFUND_PROCESSING";

        /// <summary>
        /// 退款成功
        /// </summary>
        public const string Success = "REFUND_SUCCESS";

        /// <summary>
        /// 退款失败
        /// </summary>
        public const string Fail = "REFUND_FAIL";
    }
}
