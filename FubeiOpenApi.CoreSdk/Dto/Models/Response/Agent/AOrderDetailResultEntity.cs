using System.Collections.Generic;
using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Response.Agent
{
    /// <summary>
    /// 付款码支付/订单查询
    /// http://docs.51fubei.com/agent-api/payment/orderPay.html
    /// http://docs.51fubei.com/agent-api/payment/orderQuery.html
    /// </summary>
    public class AOrderDetailResultEntity : BaseEntity
    {
        [JsonProperty("merchant_order_sn")]
        public string MerchantOrderSn { get; set; }

        [JsonProperty("order_sn")]
        public string OrderSn { get; set; }

        [JsonProperty("ins_order_sn")]
        public string InsOrderSn { get; set; }

        [JsonProperty("channel_order_sn")]
        public string ChannelOrderSn { get; set; }

        [JsonProperty("merchant_id")]
        public int MerchantId { get; set; }

        [JsonProperty("order_status")]
        public string OrderStatus { get; set; }

        [JsonProperty("pay_type")]
        public string PayType { get; set; }

        [JsonProperty("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("net_amount")]
        public decimal? NetAmount { get; set; }

        [JsonProperty("buyer_pay_amount")]
        public decimal? BuyerPayAmount { get; set; }

        [JsonProperty("fee")]
        public decimal? Fee { get; set; }

        [JsonProperty("store_id")]
        public decimal StoreId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("finish_time")]
        public string FinishTime { get; set; }

        [JsonProperty("device_no")]
        public string DeviceNo { get; set; }

        [JsonProperty("attach")]
        public string Attach { get; set; }

        [JsonProperty("payment_list")]
        public List<APaymentResultEntity> PaymentList { get; set; }

        [JsonProperty("alipay_extend_params")]
        public AAlipayExtendParamsResultEntity AlipayExtendParams { get; set; }
    }

    public class APaymentResultEntity : BaseEntity
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class AAlipayExtendParamsResultEntity : BaseEntity 
    {
        public int HbfqNum { get; set; }
    }
}
