using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class SuixingPayCallbackRequest
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

        public string scene { get; set; }

        public string buyerId { get; set; }

        public string transactionId { get; set; }

        public string payBank { get; set; }

        public string channelId { get; set; }

        public string subMechId { get; set; }

        public string finishTime { get; set; }

        public string clearDt { get; set; }

        public string storeNum { get; set; }

        public string extend { get; set; }
    }
}
