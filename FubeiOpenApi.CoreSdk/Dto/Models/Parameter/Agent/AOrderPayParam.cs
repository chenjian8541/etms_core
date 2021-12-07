using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Fubei.OpenApi.Sdk.Models;
using Newtonsoft.Json;

namespace FubeiOpenApi.CoreSdk.Models.Parameter.Agent
{
    public class AOrderPayParam : BaseEntity
    {
        
        [JsonProperty("merchant_order_sn")]
        public string MerchantOrderSn { get; set; }

        [JsonProperty("merchant_id")]
        public int? MerchantId { get; set; }

        [JsonProperty("auth_code")]
        public string AuthCode { get; set; }

        [JsonProperty("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("store_id")]
        public int StoreId { get; set; }

        [JsonProperty("cashier_id")]
        public int? CashierId { get; set; }

        [JsonProperty("sub_appid")]
        public string SubAppId { get; set; }

        [JsonProperty("goods_tag")]
        public string GoodsTag { get; set; }

        [JsonProperty("detail")]
        public AOrderPayGoodsDetailEntity Detail { get; set; }

        [JsonProperty("device_no")]
        public string DeviceNo { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("attach")]
        public string Attach { get; set; }

        [JsonProperty("timeout_express")]
        public string TimeoutExpires { get; set; }

        [JsonProperty("notify_url")]
        public string NotifyUrl { get; set; }

        [JsonProperty("alipay_extend_params")]
        public AOrderPayAlipayExtendParamsEntity AlipayExtendParams { get; set; }
    }

    public class AOrderPayGoodsDetailEntity : BaseEntity
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

    public class AOrderPayDetailEntity : BaseEntity
    {
        [JsonProperty("cost_price")]
        public int? CostPrice { get; set; }

        [JsonProperty("receipt_id")]
        public string ReceiptId { get; set; }

        [JsonProperty("goods_detail")]
        public string GoodsDetail { get; set; }
    }

    public class AOrderPayAlipayExtendParamsEntity : BaseEntity
    {
        [JsonProperty("hb_fq_instalment")] 
        public int? HbfqInstalment { get; set; }

        [JsonProperty("hb_fq_num")] 
        public int HbfqNum { get; set; }

        [JsonProperty("hb_fq_seller_percent")] 
        public int? hbfqSellerPercent { get; set; }
    }
}
