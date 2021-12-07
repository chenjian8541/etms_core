using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Response.Agent
{
    /// <summary>
    /// 统一下单接口响应
    /// </summary>
    public class AOrderCreateResultEntity : BaseEntity
    {
        /// <summary>
        /// 付呗订单号
        /// </summary>
        [JsonProperty("order_sn")]
        public string OrderSn { get; set; }

        /// <summary>
        /// 外部系统订单号
        /// </summary>
        [JsonProperty("merchant_order_sn")]
        public string MerchantOrderSn { get; set; }

        /// <summary>
        /// 预支付凭证，微信预支付订单号prepay_id、支付宝交易号tradeNo等
        /// </summary>
        [JsonProperty("prepay_id")]
        public string PrepayId { get; set; }

        /// <summary>
        /// 付呗商户号
        /// </summary>
        [JsonProperty("merchant_id")]
        public int MerchantId { get; set; }

        /// <summary>
        /// 支付方式，wxpay 微信，alipay 支付宝
        /// </summary>
        [JsonProperty("pay_type")]
        public string PayType { get; set; }

        /// <summary>
        /// 订单金额，精确到0.01
        /// </summary>
        [JsonProperty("total_amount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 商户门店号
        /// </summary>
        [JsonProperty("store_id")]
        public int StoreId { get; set; }

        /// <summary>
        /// 收银员ID
        /// </summary>
        [JsonProperty("cashier_id")]
        public int CashierId { get; set; }

        /// <summary>
        /// 付款用户id，“微信openid”、“支付宝账户”等
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        [JsonProperty("device_no")]
        public string DeviceNo { get; set; }

        /// <summary>
        /// 附加数据，原样返回，该字段主要用于商户携带订单的自定义数据
        /// </summary>
        [JsonProperty("attach")]
        public string Attach { get; set; }

        /// <summary>
        /// 签名包，当pay_type为wxpay时才返回该字段
        /// </summary>
        [JsonProperty("sign_package")]
        public AOrderCreateSignPackageEntity SignPackage { get; set; }
    }

    public class AOrderCreateSignPackageEntity : BaseEntity
    {
        /// <summary>
        /// 公众号id
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// 时间戳，示例：1414561699，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数。
        /// </summary>
        [JsonProperty("timeStamp")]
        public string TimeStamp { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        [JsonProperty("nonceStr")]
        public string NonceStr { get; set; }

        /// <summary>
        /// 统一下单接口返回的prepay_id参数值，提交格式如：prepay_id=123456
        /// </summary>
        [JsonProperty("package")]
        public string Package { get; set; }

        /// <summary>
        /// 签名类型，默认为RSA
        /// </summary>
        [JsonProperty("signType")]
        public string SignType { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [JsonProperty("paySign")]
        public string PaySign { get; set; }
    }
}