using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 刷卡（条码）支付返回信息
    /// </summary>
    [Serializable]
    public class ResponseBarcodePay:BaseResult
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
        /// 终端交易时间，yyyyMMddHHmmss，全局统一时间格式
        /// </summary>
        public string terminal_time { get; set; }

        /// <summary>
        /// 金额，单位分
        /// </summary>
        public string total_fee { get; set; }

        /// <summary>
        /// 支付完成时间，yyyyMMddHHmmss，全局统一时间格式
        /// </summary>
        public string end_time { get; set; }

        /// <summary>
        /// 利楚唯一订单号
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 通道订单号，微信订单号、支付宝订单号等，返回时不参与签名
        /// </summary>
        public string channel_trade_no { get; set; }

        /// <summary>
        /// 银行渠道订单号，微信支付时显示在支付成功页面的条码，可用作扫码查询和扫码退款时匹配
        /// </summary>
        public string channel_order_no { get; set; }

        /// <summary>
        /// 付款方用户id，“微信openid”、“支付宝账户”、“qq号”等，返回时不参与签名
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 附加数据,原样返回
        /// </summary>
        public string attach { get; set; }

        /// <summary>
        /// 口碑实收金额
        /// </summary>
        public string receipt_fee { get; set; }

    }
}
