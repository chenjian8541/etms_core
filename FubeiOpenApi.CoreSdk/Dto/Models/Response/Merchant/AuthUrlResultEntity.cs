using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Response.Merchant
{
    public class AuthUrlResultEntity : BaseEntity
    {
        [JsonProperty("authUrl")]
        public string AuthUrl { get; set; }
    }
}