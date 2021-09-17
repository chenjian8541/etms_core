using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 退款申请 返回信息
    /// </summary>
    [Serializable]
    public class ResponseUnifiedOrder : BaseResult
    {
        /// <summary>
        /// 请求类型
        /// </summary>
        public string pay_type { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string merchant_name { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }
        /// <summary>
        /// 终端号
        /// </summary>
        public string terminal_id { get; set; }
        /// <summary>
        /// 终端流水号，商户系统的订单号，扫呗系统原样返回
        /// </summary>
        public string terminal_trace { get; set; }
        /// <summary>
        /// 终端退款时间，yyyyMMddHHmmss，全局统一时间格式
        /// </summary>
        public string terminal_time { get; set; }
        /// <summary>
        /// 退款金额，单位分
        /// </summary>
        public string total_fee { get; set; }
        /// <summary>
        /// 利楚唯一订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 微信公众号支付返回字段，公众号id
        /// </summary>
        public string appId { get; set; }
        /// <summary>
        /// 微信公众号支付返回字段，时间戳
        /// </summary>
        public string timeStamp { get; set; }
        /// <summary>
        /// 微信公众号支付返回字段，随机字符串
        /// </summary>
        public string nonceStr { get; set; }
        /// <summary>
        /// 微信公众号支付返回字段，订单详情扩展字符串
        /// </summary>
        public string package_str { get; set; }
        /// <summary>
        /// 微信公众号支付返回字段，签名方式，示例：MD5,RSA
        /// </summary>
        public string signType { get; set; }
        /// <summary>
        /// 微信公众号支付返回字段，签名
        /// </summary>
        public string paySign { get; set; }
        /// <summary>
        /// 支付宝JSAPI支付返回字段用于调起支付宝JSAPI
        /// </summary>
        public string ali_trade_no { get; set; }
        /// <summary>
        /// qq钱包公众号支付
        /// </summary>
        public string token_id { get; set; }
    }
}
