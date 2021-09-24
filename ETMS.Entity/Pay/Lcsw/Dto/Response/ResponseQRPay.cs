using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 聚合支付返回类型
    /// </summary>
   public class ResponseQRPay:BaseResult
    {
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
        /// 二维码短链接
        /// </summary>
        public string qr_url { get; set; }
    }
}
