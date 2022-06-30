using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.ExternalDto.Request
{
    public class RefundReq
    {
        public string mno { get; set; }

        public string ordNo { get; set; }

        public string origUuid { get; set; }

        public decimal amt { get; set; }

        public string notifyUrl { get; set; }

        public string refundReason { get; set; }

        public string extend { get; set; }
    }
}
