using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request
{
    /// <summary>
    /// 退款申请(参数不能为空)
    /// </summary>
    public class RequestRefund : RequestPayBase
    {
        /******************************** 必填参数 ==========================*/
        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }

        public string payType { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string terminal_id { get; set; }

        /// <summary>
        /// 终端退款流水号，填写商户系统的退款流水号
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
        /// 订单号，查询凭据，利楚订单号、微信订单号、支付宝订单号任意一个
        /// </summary>
        public string out_trade_no { get; set; }

        /*========================== 必填参数 ********************************/
    }
}
