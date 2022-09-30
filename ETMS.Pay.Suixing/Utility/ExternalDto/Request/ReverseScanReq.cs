using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.ExternalDto.Request
{
    public class ReverseScanReq
    {
        public string mno { get; set; }

        public string ordNo { get; set; }

        public string amt { get; set; }

        public string authCode { get; set; }

        public string subject { get; set; }

        public string trmIp { get; set; } = "172.0.0.1";

        public string notifyUrl { get; set; }

        public string extend { get; set; }
    }
}
