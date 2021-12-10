using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Response.Agent
{
    public class AOrderRefundResultEntity : BaseEntity
    {
        public string order_sn { get; set; }

        public string refund_sn { get; set; }

        public string merchant_order_sn { get; set; }

        public string merchant_refund_sn { get; set; }

        public decimal refund_amount { get; set; }

        /// <summary>
        /// <see cref="Com.Fubei.OpenApi.Sdk.Dto.Em.EmRefundStatus"/>
        /// </summary>
        public string refund_status { get; set; }
    }
}
