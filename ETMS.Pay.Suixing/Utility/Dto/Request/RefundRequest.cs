using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto.Request
{
    public class RefundRequest
    {
        public string ordNo { get; set; }

        public string mno { get; set; }

        public string origOrderNo { get; set; }

        public string amt { get; set; }

        public string notifyUrl { get; set; }

        public string refundReason { get; set; }

        public string extend { get; set; }
    }
}
