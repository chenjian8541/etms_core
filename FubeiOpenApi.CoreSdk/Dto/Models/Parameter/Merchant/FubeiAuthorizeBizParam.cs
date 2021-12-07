using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Parameter.Merchant
{
    public class FubeiAuthorizeParam : BaseEntity
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("store_id")]
        public int StoreId { get; set; }
    }
}