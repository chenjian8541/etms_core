using Newtonsoft.Json;

namespace Com.Fubei.OpenApi.Sdk.Models
{
    public class FubeiRequestParam : BaseEntity
    {
        [JsonProperty("vendor_sn")]
        public string VendorSn { get; set; }
        
        [JsonProperty("app_id")]
        public string AppId { get; set; }
        
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("format")]
        public string Format => "json";

        [JsonProperty("sign_method")]
        public string SignMethod => "md5";
        
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("version")]
        public string Version => "1.0";
        
        [JsonProperty("biz_content")]
        public string BizContent { get; set; }
        
        [JsonProperty("sign")]
        public string Sign { get; set; }
        
        [JsonIgnore]
        public string AppSecret { get; set; }
    }
}