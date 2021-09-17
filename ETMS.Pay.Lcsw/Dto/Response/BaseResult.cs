using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 返回基类 所有返回类都应继承此类
    /// </summary>
    [Serializable]
    public class BaseResult
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public ReturnCode return_code { get; set; }

        /// <summary>
        /// 返回信息提示
        /// </summary>
        public string return_msg { get; set; }

        /// <summary>
        /// 签名字符串,拼装所有传递参数，UTF-8编码，32位md5加密转换
        /// </summary>
        public string key_sign { get; set; }

        /// <summary>
        /// 业务结果
        /// ResultCode
        /// </summary>
        public string result_code { get; set; }
    }
}
