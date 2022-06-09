using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto.Request
{
    public class JsapiScanRequest
    {
        public string extend { get; set; }

        public string ordNo { get; set; }

        public string mno { get; set; }

        public string amt { get; set; }

        /// <summary>
        /// <see cref="EmPayType"/>
        /// </summary>
        public string payType { get; set; }

        public string payWay { get; set; }

        public string subject { get; set; }

        public string trmIp { get; set; }

        public string userId { get; set; }

        public string subAppid { get; set; }

        public string notifyUrl { get; set; }
    }
}
