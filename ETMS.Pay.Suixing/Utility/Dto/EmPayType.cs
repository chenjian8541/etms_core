using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto
{
    public struct EmPayType
    {
        /// <summary>
        /// 微信
        /// </summary>
        public const string WeChat = "WECHAT";

        /// <summary>
        /// 支付宝
        /// </summary>
        public const string Alipay = "ALIPAY";

        /// <summary>
        /// 银联
        /// </summary>
        public const string UnionPay = "UNIONPAY";
    }
}
