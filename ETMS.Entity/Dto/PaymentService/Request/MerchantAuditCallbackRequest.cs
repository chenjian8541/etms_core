using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class MerchantAuditCallbackRequest
    {
        public string return_code { get; set; }

        public string return_msg { get; set; }

        public string trace_no { get; set; }

        public string result_code { get; set; }

        public string inst_no { get; set; }

        public string merchant_no { get; set; }

        public string key_sign { get; set; }
    }
}
