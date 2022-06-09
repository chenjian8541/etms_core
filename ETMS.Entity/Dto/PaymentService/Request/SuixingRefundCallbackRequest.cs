using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class SuixingRefundCallbackRequest
    {
        public string bizCode { get; set; }

        public string bizMsg { get; set; }

        public string timeStamp { get; set; }

        public string sign { get; set; }

        public string mno { get; set; }

        public string ordNo { get; set; }

        public string uuid { get; set; }

        public string payTime { get; set; }

        public string amt { get; set; }

        public string payType { get; set; }

        public string payWay { get; set; }

        public string ylTrmNo { get; set; }

        public string origOrdNo { get; set; }

        public string origUuid { get; set; }

        public string scene { get; set; }

        public string buyerId { get; set; }

        public string buyerAccount { get; set; }

        public string transactionId { get; set; }

        public string drType { get; set; }

        public string totalOffstAmt { get; set; }

        public string settleAmt { get; set; }

        public string payBank { get; set; }

        public string channelId { get; set; }

        public string subMechId { get; set; }

        public string refBuyerAmt { get; set; }

        public string openid { get; set; }

        public string sxfUuid { get; set; }

        public string extend { get; set; }

        public string storeNum { get; set; }
    }
}
