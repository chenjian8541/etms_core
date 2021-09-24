
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto
{
    /// <summary>
    /// 响应码
    /// </summary>
    public struct ReturnCode
    {
        public const string 成功 = "01";

        public const string 失败 = "02";
    }

    /// <summary>
    /// 业务结果
    /// </summary>
    public struct ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const string SUCCESS = "01";
        /// <summary>
        /// 失败
        /// </summary>
        public const string ERROR = "02";
        /// <summary>
        /// 支付中
        /// </summary>
        public const string PAYING = "03";
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

    public struct MerchantStatus
    {
        /// <summary>
        /// 商户审核通过
        /// </summary>
        public const string AuditPass = "01";

        /// <summary>
        /// 商户审核驳回
        /// </summary>
        public const string AuditFail = "02";

        /// <summary>
        /// 审核中
        /// </summary>
        public const string Auditing = "03";

        /// <summary>
        /// 审核通过且已签署电子协议
        /// </summary>
        public const string AuditPassAndSigned = "05";

        public static bool IsPass(string t)
        {
            return t == AuditPass || t == AuditPassAndSigned;
        }
    }
}
