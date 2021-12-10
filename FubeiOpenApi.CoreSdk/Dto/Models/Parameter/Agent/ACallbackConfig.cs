using System;
using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Parameter.Agent
{
    /// <summary>
    /// https://www.yuque.com/51fubei/openapi/payment_callbackconfig
    /// </summary>
    public class ACallbackConfig : BaseEntity
    {
        public int? merchant_id { get; set; }

        public string second_callback_url { get; set; }

        public string remit_callback_url { get; set; }

        public string refund_callback_url { get; set; }
    }
}
