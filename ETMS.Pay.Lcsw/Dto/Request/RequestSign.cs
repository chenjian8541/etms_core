using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto.Request
{
    /// <summary>
    /// 注册终端(参数不能为空)
    /// </summary>
    public class RequestSign
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string terminal_id { get; set; }

        /// <summary>
        /// 终端流水号，填写商户系统的订单号
        /// </summary>
        public string terminal_trace { get; set; }

        /// <summary>
        /// 终端注册时间，yyyyMMddHHmmss，全局统一时间格式
        /// </summary>
        public string terminal_time { get; set; }
    }
}
