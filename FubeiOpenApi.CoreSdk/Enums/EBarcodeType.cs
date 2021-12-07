using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Fubei.OpenApi.Sdk.Enums
{
    /// <summary>
    /// BarCode类型
    /// </summary>
    public enum EBarcodeType
    {
        /// <summary>
        /// 未确定的付款码
        /// </summary>
        Undetermined = 0,
        
        /// <summary>
        /// 微信付款码
        /// </summary>
        Wechat = 1,

        /// <summary>
        /// 支付宝付款码
        /// </summary>
        Alipay = 2,

        /// <summary>
        /// 银联付款码
        /// </summary>
        UnionPay = 5,
    }
}
