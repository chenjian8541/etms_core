using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class FubeiApiNotifyRequest
    {
        public string ResultCode { get; set; }

        public string ResultMessage { get; set; }

        public string Data { get; set; }

        public new string AppSecret { get; set; }

        public string Sign { get; set; }
    }

    public class FubeiApiNotifyData {
        public string order_sn { get; set; }

        public string merchant_order_sn { get; set; }

        public string ins_order_sn { get; set; }

        public string channel_order_sn { get; set; }

        public string order_status { get; set; }

        public string pay_type { get; set; }

        public float total_amount { get; set; }

        public float net_amount { get; set; }

        public float buyer_pay_amount { get; set; }

        public float fee { get; set; }

        public int store_id { get; set; }

        public int cashier_id { get; set; }

        public string user_id { get; set; }

        public string finish_time { get; set; }

        public string device_no { get; set; }

        public string attach { get; set; }
    }
}
