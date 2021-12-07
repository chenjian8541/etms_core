using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FubeiOpenApi.CoreSdk.Models.Parameter.Agent
{
    public class AOrderQueryParam : BaseEntity
    {
        [JsonProperty("merchant_id")]
        public int? MerchantId { get; set; }

        [JsonProperty("order_sn")]
        public string OrderSn { get; set; }

        [JsonProperty("merchant_order_sn")]
        public string MerchantOrderSn { get; set; }

        [JsonProperty("ins_order_sn")]
        public string InsOrderSn { get; set; }
    }
}
