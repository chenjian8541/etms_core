using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request
{
    /// <summary>
    /// 查询清算（参数不能为空）
    /// </summary>
    [Serializable]
    public class RequestGetSettlement
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }
        /// <summary>
        /// 查询清算开始时间（yyyyMMdd）
        /// </summary>
        public string begin_date { get; set; }
        /// <summary>
        /// 查询清算结束时间（yyyyMMdd）
        /// </summary>
        public string end_date { get; set; }
    }
    
}
