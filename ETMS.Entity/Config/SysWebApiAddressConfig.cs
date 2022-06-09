﻿using System;
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

        public static string SuixingPayCallbackUrl;

        public static string SuixingRefundCallbackUrl;

        public static void InitConfig(string baseUrl)
        {
            BaseUrl = baseUrl;
            MerchantAuditCallbackUrl = $"{baseUrl}/pay/merchantAuditCallback";
            LcsPayJspayCallbackUrl = $"{baseUrl}/pay/lcsPayJspayCallback";
            FubeiApiNotifyUrl = $"{baseUrl}/pay/FubeiApiNotify";
            FubeiRefundApiNotify = $"{baseUrl}/pay/FubeiRefundApiNotify";
            SuixingPayCallbackUrl = $"{baseUrl}/pay/SuixingPayCallback";
            SuixingRefundCallbackUrl = $"{baseUrl}/pay/SuixingRefundCallback";
        }
    }
}
