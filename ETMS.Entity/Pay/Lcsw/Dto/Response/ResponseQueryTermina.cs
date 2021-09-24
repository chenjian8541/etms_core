using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 查询终端返回信息
    /// </summary>
    [Serializable]
    public class ResponseQueryTermina : BaseResult
    {
        /// <summary>
        /// 请求流水号
        /// </summary>
        public string trace_no { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string merchant_name { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        public string store_code { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string store_name { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string terminal_id { get; set; }

        /// <summary>
        /// 终端令牌
        /// </summary>
        public string access_token { get; set; }

    }
}
