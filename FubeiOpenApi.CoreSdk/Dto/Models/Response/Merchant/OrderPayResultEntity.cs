using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Response.Merchant
{
    public class OrderPayResultEntity : BaseEntity
    {
        [JsonProperty("merchant_order_sn")]
        public string MerchantOrderSn { get; set; }

        [JsonProperty("order_sn")]
        public string OrderSn { get; set; }

        [JsonProperty("trade_no")]
        public string TradeNo { get; set; }

        [JsonProperty("trade_state")]
        public string TradeState { get; set; }

        [JsonProperty("total_fee")]
        public decimal TotalFee { get; set; }

        [JsonProperty("order_price")]
        public decimal OrderPrice { get; set; }

        [JsonProperty("pay_time")]
        public int PayTime { get; set; }

        [JsonProperty("type")]
        public int? Type { get; set; }

        [JsonProperty("discount_money")]
        public decimal? DiscountMoney { get; set; }

        [JsonProperty("buyer_pay_amount")]
        public decimal? BuyerPayAmount { get; set; }

        [JsonProperty("attach")]
        public string Attach { get; set; }

        [JsonProperty("store_id")]
        public int StoreId { get; set; }

        [JsonProperty("cashier_id")]
        public int? CashierId { get; set; }

        [JsonProperty("device_no")]
        public string DeviceNo { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("user_logon_id")]
        public string UserLogonId { get; set; }

        [JsonProperty("cash_coupon_fee")]
        public decimal? CashCouponFee { get; set; }

        [JsonProperty("no_cash_coupon_fee")]
        public decimal? NoCashCouponFee { get; set; }

        [JsonProperty("fee")]
        public decimal? Fee { get; set; }

        [JsonProperty("platform_order_no")]
        public string PlatformOrderNo { get; set; }

    }
}
