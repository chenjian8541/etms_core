using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto.Response
{
    public class JsapiScanResponse
    {
        public string bizCode { get; set; }

        public string bizMsg { get; set; }

        public string ordNo { get; set; }

        public string uuid { get; set; }

        public string sxfUuid { get; set; }

        public string prepayId { get; set; }

        public string payAppId { get; set; }

        public string payTimeStamp { get; set; }

        public string paynonceStr { get; set; }

        public string payPackage { get; set; }

        public string paySignType { get; set; }

        public string paySign { get; set; }

        public string partnerId { get; set; }

        public string redirectUrl { get; set; }

        public string source { get; set; }
    }
}
