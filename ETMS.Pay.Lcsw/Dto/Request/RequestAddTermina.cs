using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto.Request
{
    /// <summary>
    /// 创建终端(参数不能为空)
    /// </summary>
    public class RequestAddTermina
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }

        /// <summary>
        /// 请求流水号
        /// </summary>
        public string trace_no { get; set; }

    }
}
