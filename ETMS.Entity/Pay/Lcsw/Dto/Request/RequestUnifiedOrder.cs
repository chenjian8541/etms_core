using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request
{
    /// <summary>
    /// 统一下单
    /// </summary>
    public class RequestUnifiedOrder : RequestPayBase
    {
        /******************************** 必填参数 ==========================*/
        public string payType { get; set; }

        public string notify_url { get; set; }

        public string open_id { get; set; }

        /// <summary>
        /// 终端退款流水号，填写商户系统的订单号
        /// 必填
        /// </summary>
        public string terminal_trace { get; set; }

        /// <summary>
        /// 金额，单位分
        /// 必填
        /// </summary>
        public string total_fee { get; set; }

        /*========================== 必填参数 ********************************/

        /*************************** 选填参数 ===============================*/
        /// <summary>
        /// 订单描述
        /// 选填
        /// </summary>
        public string order_body { get; set; }

        /// <summary>
        /// 附加数据，原样返回
        /// 选填
        /// </summary>
        public string attach { get; set; }

        public string sub_appid { get; set; }
    }
}
