using Com.Fubei.OpenApi.Sdk.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Parameter
{
    public class FubeiNotificationParam : BaseEntity
    {
        [JsonProperty("result_code")]
        [FromForm(Name = "result_code")]
        public string ResultCode { get; set; }

        [JsonProperty("result_message")]
        [FromForm(Name = "result_message")]
        public string ResultMessage { get; set; }

        [JsonProperty("data")]
        [FromForm(Name = "data")]
        public string Data { get; set; }

        [JsonIgnore]
        public new string AppSecret { get; set; }
    }
}