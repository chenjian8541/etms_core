using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Config
{
    public class SysWebApiAddressConfig
    {
        public static string BaseUrl;

        public static string MerchantAuditCallbackUrl;

        public static string LcsPayJspayCallbackUrl;

        public static string FubeiApiNotifyUrl;

        public static string FubeiRefundApiNotify;

        /// <summary>
        /// 处理小程序支付成功回调
        /// </summary>
        public static string SuixingPayCallbackUrl;

        /// <summary>
        /// 处理其他支付成功回调
        /// </summary>
        public static string SuixingPayCallbackUrl2;

        public static string SuixingRefundCallbackUrl;

        public static void InitConfig(string baseUrl)
        {
            BaseUrl = baseUrl;
            MerchantAuditCallbackUrl = $"{baseUrl}/pay/merchantAuditCallback";
            LcsPayJspayCallbackUrl = $"{baseUrl}/pay/lcsPayJspayCallback";
            FubeiApiNotifyUrl = $"{baseUrl}/pay/FubeiApiNotify";
            FubeiRefundApiNotify = $"{baseUrl}/pay/FubeiRefundApiNotify";
            SuixingPayCallbackUrl = $"{baseUrl}/pay/SuixingPayCallback";
            SuixingPayCallbackUrl2 = $"{baseUrl}/pay/SuixingPayCallback2";
            SuixingRefundCallbackUrl = $"{baseUrl}/pay/SuixingRefundCallback";
        }
    }
}
