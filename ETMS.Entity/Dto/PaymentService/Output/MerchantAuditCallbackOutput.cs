using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class MerchantAuditCallbackOutput
    {
        public string return_code { get; set; }

        public string return_msg { get; set; }

        public string trace_no { get; set; }
    }
}
