using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request
{
    /// <summary>
    /// 刷卡（条码）支付(参数不能为空)
    /// </summary>
    public class RequestBarcodePay : RequestPayBase
    {
        /******************************** 必填参数 ==========================*/
        public string payType { get; set; }

        /// <summary>
        /// 终端流水号，填写商户系统的订单号
        /// </summary>
        public string terminal_trace { get; set; }

        /// <summary>
        /// 授权码
        /// </summary>
        public string auth_no { get; set; }

        /// <summary>
        /// 金额，单位分
        /// </summary>
        public string total_fee { get; set; }

        public string order_body { get; set; }

        public string attach { get; set; }

        /*========================== 必填参数 ********************************/
    }
}
