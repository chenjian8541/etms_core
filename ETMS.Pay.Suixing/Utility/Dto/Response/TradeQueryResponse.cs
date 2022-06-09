using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto.Response
{
    public class TradeQueryResponse
    {
        public string bizCode { get; set; }

        public string bizMsg { get; set; }

        public string ordNo { get; set; }

        public string uuid { get; set; }

        public string payTime { get; set; }

        public string tranTime { get; set; }

        public string oriTranAmt { get; set; }

        public string ledgerAccountFlag { get; set; }

        public string payType { get; set; }

        public string payWay { get; set; }

        public string subject { get; set; }

        /// <summary>
        /// <see cref="EmTranSts"/>
        /// </summary>
        public string tranSts { get; set; }

        public string sxfUuid { get; set; }

        public string finishTime { get; set; }

        public string clearDt { get; set; }

        public string deviceNo { get; set; }

        public string tradeCode { get; set; }

        public string szltRecfeeAmt { get; set; }

        public string payBank { get; set; }

        public string storeName { get; set; }

        public string totalOffstAmt { get; set; }
    }
}
