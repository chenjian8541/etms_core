using System;
using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Parameter.Agent
{
    /// <summary>
    /// 微信参数配置
    /// https://www.yuque.com/51fubei/openapi/payment_wxconfig
    /// </summary>
    public class AOrderWxConfig : BaseEntity
    {
        /// <summary>
        /// 付呗商户号，以服务商级接入时必传，以商户级接入时不传
        /// </summary>
        [JsonProperty("merchant_id")]
        public int? MerchantId { get; set; }

        /// <summary>
        /// 付呗系统的门店id
        /// </summary>
        [JsonProperty("store_id")]
        public int StoreId { get; set; }

        /// <summary>
        /// 支付所使用的公众号appid， 支持使用小程序app_id
        /// </summary>
        [JsonProperty("sub_appid")]
        public string SubAppId { get; set; }

        /// <summary>
        /// 支付授权目录
        /// </summary>
        [JsonProperty("jsapi_path")]
        public string JsapiPath { get; set; }
    }
}