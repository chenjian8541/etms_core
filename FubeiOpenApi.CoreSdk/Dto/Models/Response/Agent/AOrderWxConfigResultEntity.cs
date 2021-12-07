using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Response.Agent
{
    /// <summary>
    /// 微信参数配置 返回值
    /// https://www.yuque.com/51fubei/openapi/payment_wxconfig
    /// </summary>
    public class AOrderWxConfigResultEntity : BaseEntity
    {
        /// <summary>
        /// 付呗商户号
        /// </summary>
        [JsonProperty("merchant_id")]
        public int MerchantId { get; set; }

        /// <summary>
        /// 付呗系统的门店id
        /// </summary>
        [JsonProperty("store_id")]
        public int StoreId { get; set; }

        /// <summary>
        /// 支付APPID配置结果：1 成功 2 失败
        /// </summary>
        [JsonProperty("sub_appid_code")]
        public int SubAppidCode { get; set; }

        /// <summary>
        /// 支付APPID响应描述
        /// </summary>
        [JsonProperty("sub_appid_msg")]
        public string SubAppidMsg { get; set; }

        /// <summary>
        /// 支付授权目录配置结果：1 成功、2 失败
        /// </summary>
        [JsonProperty("jsapi_code")]
        public int JsapiCode { get; set; }

        /// <summary>
        /// 支付授权目录响应描述
        /// </summary>
        [JsonProperty("jsapi_msg")]
        public string JsapiMsg { get; set; }
    }
}