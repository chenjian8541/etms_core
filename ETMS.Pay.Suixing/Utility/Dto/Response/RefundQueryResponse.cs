using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto.Response
{
    public class RefundQueryResponse
    {
        public string bizCode { get; set; }

        public string bizMsg { get; set; }

        public string uuid { get; set; }

        public string ordNo { get; set; }

        /// <summary>
        /// <see cref="EmRefundTranSts"/>
        /// </summary>
        public string tranSts { get; set; }

        public string refundAmount { get; set; }

        public string totalAmount { get; set; }

        public string sxfUuid { get; set; }

        public string transactionId { get; set; }

        public string payTime { get; set; }

        public string reconKey { get; set; }

        public string realRefundAmount { get; set; }

        public string origOrdNo { get; set; }

        public string origUuid { get; set; }

        public string extend { get; set; }

        public string refundReason { get; set; }

        public string payType { get; set; }

        public string finishTime { get; set; }

        public string storeNum { get; set; }
    }
}
