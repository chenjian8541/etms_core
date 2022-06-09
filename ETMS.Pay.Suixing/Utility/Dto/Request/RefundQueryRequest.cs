using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto.Request
{
    public class RefundQueryRequest
    {
        /// <summary>
        /// 退款订单号
        /// </summary>
        public string ordNo { get; set; }

        public string mno { get; set; }
    }
}
