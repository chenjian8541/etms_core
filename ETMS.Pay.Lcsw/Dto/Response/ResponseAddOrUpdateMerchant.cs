using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 添加/更新商户 返回信息
    /// </summary>
    [Serializable]
    public class ResponseAddOrUpdateMerchant : BaseResult
    {

        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }       

    }
}
