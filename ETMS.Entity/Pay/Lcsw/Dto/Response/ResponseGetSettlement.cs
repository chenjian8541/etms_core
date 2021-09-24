using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 查询清算 返回信息
    /// </summary>
    [Serializable]
    public class ResponseGetSettlement:BaseResult
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
        /// 清算结果集
        /// </summary>
        public string settle_list { get; set; }
    }

    /// <summary>
    /// 清算结果
    /// </summary>
    [Serializable]
    public class SettleResult
    {
        /// <summary>
        /// 清算日期（yyyyMMdd）
        /// </summary>
        public string settle_date { get; set; }
        /// <summary>
        /// 清算金额（分）
        /// </summary>
        public long settle_amt { get; set; }
        /// <summary>
        /// 清算状态
        /// </summary>
        public int settle_status { get; set; }
        /// <summary>
        /// 清算状态描述
        /// </summary>
        public string settle_status_msg { get; set; }
    }
}
