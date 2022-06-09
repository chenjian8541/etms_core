using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto
{
    public struct EmTranSts
    {
        /// <summary>
        /// 交易成功
        /// </summary>
        public const string SUCCESS = "SUCCESS";

        /// <summary>
        /// 交易失败
        /// </summary>
        public const string FAIL = "FAIL";

        /// <summary>
        /// 支付中
        /// </summary>
        public const string PAYING = "PAYING";

        /// <summary>
        /// 已关闭
        /// </summary>
        public const string CLOSED = "CLOSED";

        /// <summary>
        /// 已撤销
        /// </summary>
        public const string CANCELED = "CANCELED ";
    }
}
