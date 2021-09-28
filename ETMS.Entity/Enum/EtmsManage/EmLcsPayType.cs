using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmLcsPayType
    {
        /// <summary>
        /// 微信
        /// </summary>
        public const string WeChat = "010";

        /// <summary>
        /// 支付宝
        /// </summary>
        public const string Alipay = "020";

        /// <summary>
        /// qq钱包
        /// </summary>
        public const string QQ = "060";

        /// <summary>
        /// 京东钱包
        /// </summary>
        public const string JDPay = "080";

        /// <summary>
        /// 口碑
        /// </summary>
        public const string PublicPraisePay = "090";

        /// <summary>
        /// 翼支付
        /// </summary>
        public const string Bestpay = "100";

        /// <summary>
        /// 银联二维码
        /// </summary>
        public const string UnionPayQRCode = "110";

        /// <summary>
        /// 和包支付（仅限和包通道）
        /// </summary>
        public const string PackagePay = "140";

        /// <summary>
        /// 自动识别类型
        /// </summary>
        public const string AutoTypePay = "000";

        public static string GetPayTypeDesc(string t)
        {
            switch (t)
            {
                case WeChat:
                    return "微信";
                case Alipay:
                    return "支付宝";
                case QQ:
                    return "qq钱包";
                case JDPay:
                    return "京东钱包";
                case PublicPraisePay:
                    return "口碑";
                case Bestpay:
                    return "翼支付";
                case UnionPayQRCode:
                    return "银联二维码";
                case PackagePay:
                    return "和包支付";
            }
            return string.Empty;
        }
    }
}
