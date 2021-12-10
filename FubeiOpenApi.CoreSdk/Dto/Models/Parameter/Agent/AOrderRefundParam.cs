using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Parameter.Agent
{
    public class AOrderRefundParam : BaseEntity
    {
        public int? merchant_id { get; set; }

        public string order_sn { get; set; }

        public string merchant_order_sn { get; set; }

        public string merchant_refund_sn { get; set; }

        public decimal refund_amount { get; set; }
    }
}
