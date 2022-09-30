using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto.Response
{
    public class ReverseScanResponse
    {
        public string bizCode { get; set; }

        public string bizMsg { get; set; }

        public string tranSts { get; set; }

        public string ordNo { get; set; }

        public string uuid { get; set; }

        public string payTime { get; set; }

        public string oriTranAmt { get; set; }

        public string payType { get; set; }

        public string payWay { get; set; }

        public string transactionId { get; set; }

        public string subAppid { get; set; }

        public string channelId { get; set; }

        public string buyerId { get; set; }

        public string buyerAccount { get; set; }

        public string scene { get; set; }

        public string drType { get; set; }

        public string recFeeRate { get; set; }

        public string extend { get; set; }

        public string ylTrmNo { get; set; }
    }
}
