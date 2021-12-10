using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class FubeiApiNotifyRequestData
    {
        public string order_sn { get; set; }

        public string refund_sn { get; set; }

        public string merchant_order_sn { get; set; }

        public string merchant_refund_sn { get; set; }

        public decimal refund_amount { get; set; }

        public decimal buyer_refund_amount { get; set; }

        public string refund_status { get; set; }

        public string finish_time { get; set; }
    }
}
