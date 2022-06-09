using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto
{
    /*
     0000	操作成功
     0001	系统异常
     0002	报文格式错误
     0003	机构id异常
     0004	解密失败
     0005	验签失败
     0006	该功能未开通
     0008	未配置公钥
     0009	机构不存在
     0010	机构状态异常
     0011	该机构已不合作
     */
    public struct EmResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const string Success = "0000";
    }
}
