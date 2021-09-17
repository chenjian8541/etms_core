using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 注册终端返回信息返回信息
    /// </summary>
    [Serializable]
    public class ResponseSign : BaseResult
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
        /// 终端流水号，商户系统的订单号，扫呗系统原样返回
        /// </summary>
        public string terminal_trace { get; set; }

        /// <summary>
        /// 终端注册时间，yyyyMMddHHmmss，全局统一时间格式
        /// </summary>
        public string terminal_time { get; set; }

        /// <summary>
        /// 令牌(需要永久保存)
        /// </summary>
        public string access_token { get; set; }        

    }
}
