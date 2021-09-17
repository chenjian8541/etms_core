
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Dto
{
    /// <summary>
    /// 响应码
    /// </summary>
    public enum ReturnCode
    {
        成功 = 01,
        失败 = 02,
    }

    /// <summary>
    /// 业务结果
    /// </summary>
    public static class ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static string SUCCESS = "01";
        /// <summary>
        /// 失败
        /// </summary>
        public static string ERROR = "02";
        /// <summary>
        /// 支付中
        /// </summary>
        public static string PAYING = "03";
    }

    /// <summary>
    /// 支付类型
    /// </summary>
    public enum Pay_Type
    {

        微信 = 010,
        支付宝 = 020,
        QQ钱包 = 030,
        京东钱包 = 080,
        口碑 = 090,
        银联二维码 = 110,
    }

    /// <summary>
    /// 交易订单状态
    /// </summary>
    public static class TradeState
    {
        /// <summary>
        /// 支付成功
        /// </summary>
        public static string SUCCESS = "SUCCESS";
        /// <summary>
        /// 转入退款
        /// </summary>
        public static string REFUND = "REFUND";
        /// <summary>
        /// 未支付
        /// </summary>
        public static string NOTPAY = "NOTPAY";
        /// <summary>
        /// 已关闭
        /// </summary>
        public static string CLOSED = "CLOSED";
        /// <summary>
        /// 用户支付中
        /// </summary>
        public static string USERPAYING = "USERPAYING";
        /// <summary>
        /// 已撤销
        /// </summary>
        public static string REVOKED = "REVOKED";
        /// <summary>
        /// 未支付支付超时
        /// </summary>
        public static string NOPAY = "NOPAY";
        /// <summary>
        /// 支付失败
        /// </summary>
        public static string PAYERROR = "PAYERROR";

    }
}
