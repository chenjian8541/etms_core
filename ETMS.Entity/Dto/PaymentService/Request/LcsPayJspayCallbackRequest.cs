using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class LcsPayJspayCallbackRequest
    {
        public string return_code { get; set; }

        public string return_msg { get; set; }

        public string result_code { get; set; }

        public string pay_type { get; set; }

        public string user_id { get; set; }

        public string merchant_name { get; set; }

        public string merchant_no { get; set; }

        public string terminal_id { get; set; }

        public string terminal_trace { get; set; }

        public string terminal_time { get; set; }

        public string pay_trace { get; set; }

        public string pay_time { get; set; }

        public string total_fee { get; set; }

        public string end_time { get; set; }

        public string out_trade_no { get; set; }

        public string channel_trade_no { get; set; }

        public string attach { get; set; }

        public string receipt_fee { get; set; }

        public string key_sign { get; set; }
    }
}
