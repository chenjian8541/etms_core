using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.CoreBusiness.Output
{
    public class UnifiedOrderOutput : AgtPayServiceOutputBase
    {
        public string OutTradeNo { get; set; }

        public string PayType { get; set; }

        /// <summary>
        /// 微信公众号支付返回字段，公众号id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 微信公众号支付返回字段，时间戳
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// 微信公众号支付返回字段，随机字符串
        /// </summary>
        public string NonceStr { get; set; }

        /// <summary>
        /// 微信公众号支付返回字段，订单详情扩展字符串
        /// </summary>
        public string Package_str { get; set; }

        /// <summary>
        /// 微信公众号支付返回字段，签名方式，示例：MD5,RSA
        /// </summary>
        public string SignType { get; set; }

        /// <summary>
        /// 微信公众号支付返回字段，签名
        /// </summary>
        public string PaySign { get; set; }

        public string ali_trade_no { get; set; }
    }
}
