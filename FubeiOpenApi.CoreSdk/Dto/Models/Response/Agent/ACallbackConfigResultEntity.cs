using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Response.Agent
{
    /// <summary>
    /// https://www.yuque.com/51fubei/openapi/payment_callbackconfig
    /// </summary>
    public class ACallbackConfigResultEntity : BaseEntity
    {
        public int? merchant_id { get; set; }

        public int? agent_id { get; set; }

        public int bind_status { get; set; }

        public string resp_message { get; set; }
    }
}
