using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto.Request
{
    /// <summary>
    /// 支付查询(参数不能为空)
    /// </summary>
    public class RequestQuery : RequestPayBase
    {
        /******************************** 必填参数 ==========================*/
        public string payType { get; set; }

        public string pay_trace { get; set; }

        public string pay_time { get; set; }

        /// <summary>
        /// 终端查询流水号，填写商户系统的查询流水号
        /// </summary>
        public string terminal_trace { get; set; }

        /// <summary>
        /// 订单号，查询凭据，可填利楚订单号、微信订单号、支付宝订单号、银行卡订单号任意一个
        /// </summary>
        public string out_trade_no { get; set; }


        /*========================== 必填参数 ********************************/
    }
}
