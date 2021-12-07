using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Parameter.Merchant
{
    public class FubeiH5PayParam : BaseEntity
    {
        [JsonProperty("merchant_order_sn")]
        public string MerchantOrderSn;

        [JsonProperty("open_id")]
        public string OpenId;

        [JsonProperty("sub_open_id")]
        public string SubOpenId;

        [JsonProperty("total_fee")]
        public decimal? TotalFee;

        [JsonProperty("store_id")]
        public int? StoreId;

        [JsonProperty("cashier_id")]
        public int? CashierId;
    }
}