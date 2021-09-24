using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request
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

    }
}
