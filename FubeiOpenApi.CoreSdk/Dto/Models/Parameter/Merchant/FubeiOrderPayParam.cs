using System.Collections.Generic;
using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Parameter.Merchant
{
    /// <summary>
    /// 付呗订单支付参数
    /// </summary>
    public class OrderPayParam : BaseEntity
    {
        #region GoodDetailEntity定义
        public class GoodDetailEntity : BaseEntity
        {
            [JsonProperty("goods_id")]
            public string GoodsId { get; set; }

            [JsonProperty("goods_name")]
            public string GoodsName { get; set; }

            [JsonProperty("quantity")]
            public int Quantity { get; set; }

            [JsonProperty("price")]
            public int Price { get; set; }
        }
        #endregion

        #region GoodDetailEntity定义
        public class SwipeGoodDetailEntity : BaseEntity
        {
            [JsonProperty("cost_price")]
            public int CostPrice { get; set; }

            [JsonProperty("receipt_id	")]
            public string ReceiptId { get; set; }

            [JsonProperty("goods_detail")]
            public List<GoodDetailEntity> GoodsDetail { get; set; }
        }
        #endregion

        [JsonProperty("merchant_order_sn")]
        public string MerchantOrderSn { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("auth_code")]
        public string AuthCode { get; set; }

        [JsonProperty("total_fee")]
        public decimal TotalFee { get; set; }

        [JsonProperty("store_id")]
        public int? StoreId { get; set; }

        [JsonProperty("cashier_id")]
        public int? CashierId { get; set; }


        [JsonProperty("device_no")]
        public string DeviceNo { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("call_back_url")]
        public string CallBackUrl { get; set; }

        [JsonProperty("equipment_type")]
        public int? EquipmentType { get; set; }

        [JsonProperty("discount_switch")]
        public int? DiscountSwitch { get; set; }

        [JsonProperty("attach")]
        public string Attach { get; set; }

        [JsonProperty("goods_tag")]
        public string GoodsTag { get; set; }

        [JsonProperty("detail")]
        public GoodDetailEntity Detail { get; set; }

        [JsonProperty("sub_appid")]
        public string SubAppId { get; set; }

        [JsonProperty("timeout_express")]
        public string TimeoutExpress { get; set; }

        [JsonProperty("scene")]
        public string Scene { get; set; }

        [JsonProperty("buyer_id")]
        public string BuyerId { get; set; }
    }
}
