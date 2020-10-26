using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 收入帐支付类型
    /// </summary>
    public struct EmPayType
    {
        /// <summary>
        /// 支付宝
        /// </summary>
        public const byte Alipay = 0;

        /// <summary>
        /// 微信支付
        /// </summary>
        public const byte WeChat = 1;

        /// <summary>
        /// 现金支付
        /// </summary>
        public const byte Cash = 2;

        /// <summary>
        /// 银联支付
        /// </summary>
        public const byte Bank = 3;

        /// <summary>
        /// POS机
        /// </summary>
        public const byte Pos = 4;

        /// <summary>
        /// 其他支付
        /// </summary>
        public const byte Other = 99;

        public static string GetPayType(int b)
        {
            switch (b)
            {
                case Alipay:
                    return "支付宝";
                case WeChat:
                    return "微信支付";
                case Cash:
                    return "现金支付";
                case Bank:
                    return "银联支付";
                case Pos:
                    return "POS机";
                case Other:
                    return "其他支付";
            }
            return string.Empty;
        }

        public static string[] GetPayTypeAll()
        {
            return new string[] {
            "支付宝",
            "微信支付",
            "现金支付",
            "银联支付",
            "POS机",
            "其他支付",
            };
        }

        public static byte GetPayType(string name)
        {
            switch (name)
            {
                case "支付宝":
                    return Alipay;
                case "微信支付":
                    return WeChat;
                case "现金支付":
                    return Cash;
                case "银联支付":
                    return Bank;
                case "POS机":
                    return Pos;
                case "其他支付":
                    return Other;
            }
            return Cash;
        }
    }
}
