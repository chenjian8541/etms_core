using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 退款申请 返回信息
    /// </summary>
    [Serializable]
    public class ResponseRefund : BaseResult
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
        public string refund_fee { get; set; }

        /// <summary>
        /// 退款完成时间，yyyyMMddHHmmss，全局统一时间格式
        /// </summary>
        public string end_time { get; set; }

        /// <summary>
        /// 利楚唯一订单号
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 利楚唯一退款单号
        /// </summary>
        public string out_refund_no { get; set; }

    }
}
