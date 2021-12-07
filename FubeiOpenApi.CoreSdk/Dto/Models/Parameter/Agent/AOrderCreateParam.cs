using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Parameter.Agent
{
    /// <summary>
    /// 统一下单
    /// </summary>
    public class AOrderCreateParam : BaseEntity
    {
        /// <summary>
        /// 外部系统订单号（确保唯一，前后不允许带空格）
        /// </summary>
        [JsonProperty("merchant_order_sn")]
        public string MerchantOrderSn { get; set; }
        
        /// <summary>
        /// 付呗商户号，以服务商级接入时必传，以商户级接入时不传
        /// </summary>
        [JsonProperty("merchant_id")]
        public int? MerchantId { get; set; }
        
        /// <summary>
        /// 支付方式，wxpay:微信，alipay:支付宝
        /// </summary>
        [JsonProperty("pay_type")]
        public string PayType { get; set; }
        
        /// <summary>
        /// 订单总金额，单位为元，精确到0.01 ~ 10000000
        /// </summary>
        [JsonProperty("total_amount")]
        public decimal TotalAmount { get; set; }
        
        /// <summary>
        /// 商户门店号（如果只有一家有效门店，可不传）
        /// </summary>
        [JsonProperty("store_id")]
        public int? StoreId { get; set; }
        
        /// <summary>
        /// 收银员ID
        /// </summary>
        [JsonProperty("cashier_id")]
        public int? CashierId { get; set; }
        
        /// <summary>
        /// 公众号appid。当微信支付时，该字段必填（user_id需要保持一致，即为该公众号appid获取的）
        /// </summary>
        [JsonProperty("sub_appid")]
        public string SubAppId { get; set; }
        
        /// <summary>
        /// 用户标识（微信openid，支付宝userid）
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        
        /// <summary>
        /// 订单优惠标记，代金券或立减优惠功能的参数（使用单品券时必传）
        /// </summary>
        [JsonProperty("goods_tag")]
        public string GoodsTag { get; set; }
        
        /// <summary>
        /// 订单包含的商品信息，Json格式。当当微信支付或者支付宝支付时时可选填此字段。对于使用单品优惠的商户，该字段必须按照规范上传，详见“单品优惠参数说明”
        /// </summary>
        [JsonProperty("detail")]
        public AOrderPayGoodsDetailEntity Detail { get; set; }
        
        /// <summary>
        /// 终端号
        /// </summary>
        [JsonProperty("device_no")]
        public string DeviceNo { get; set; }
        
        /// <summary>
        /// 商品描述
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }
        
        /// <summary>
        /// 附加数据，原样返回，该字段主要用于商户携带订单的自定义数据
        /// </summary>
        [JsonProperty("attach")]
        public string Attach { get; set; }
        
        /// <summary>
        /// 订单失效时间，逾期将关闭交易。格式为yyyyMMddHHmmss，失效时间需大于1分钟。银联暂不支持
        /// </summary>
        [JsonProperty("timeout_express")]
        public string TimeoutExpires { get; set; }
        
        /// <summary>
        /// 支付回调地址
        /// </summary>
        [JsonProperty("notify_url")]
        public string NotifyUrl { get; set; }
        
        /// <summary>
        /// 支付宝业务拓展参数--花呗分期
        /// </summary>
        [JsonProperty("alipay_extend_params")]
        public AOrderPayAlipayExtendParamsEntity AlipayExtendParams { get; set; }
        
        /// <summary>
        /// 平台方门店号（即微信/支付宝的storeid）
        /// </summary>
        [JsonProperty("platform_store_id")]
        public string PlatformStoreId { get; set; }
    }
}