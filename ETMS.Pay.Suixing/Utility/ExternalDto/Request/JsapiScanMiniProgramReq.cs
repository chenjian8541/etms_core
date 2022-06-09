using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.ExternalDto.Request
{
    public class JsapiScanMiniProgramReq
    {
        public string mno { get; set; }

        public string ordNo { get; set; }

        public decimal amt { get; set; }

        public string subject { get; set; }

        public string notifyUrl { get; set; }

        public string trmIp { get; set; } = "172.0.0.1";

        public string openid { get; set; }

        /// <summary>
        /// 扩展字段 可以携带机构信息
        /// </summary>
        public string extend { get; set; }
    }
}
