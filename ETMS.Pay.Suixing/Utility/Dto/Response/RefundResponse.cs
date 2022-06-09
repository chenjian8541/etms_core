using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto.Response
{
    public class RefundResponse
    {
        public string bizCode { get; set; }

        public string bizMsg { get; set; }

        /// <summary>
        /// <see cref="EmRefundTranSts"/>
        /// </summary>
        public string tranSts { get; set; }

        public string origOrderNo { get; set; }

        public string origUuid { get; set; }

        public string uuid { get; set; }

        public string amt { get; set; }

        public string realRefundAmount { get; set; }

        public string sxfUuid { get; set; }

        public string transactionId { get; set; }

        public string buyerId { get; set; }

        public string refBuyerAmt { get; set; }

        public string finishTime { get; set; }
    }
}
