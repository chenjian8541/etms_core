using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing
{
    public static class SuiXingConfig
    {
        internal const int TIME_OUT = 10000;

        internal static string _privateKeyPem;

        internal static string _publicKeyPem;

        internal static string _orgId;

        internal static string _merchantInfoQuery;

        internal static string _jsapiScan;

        internal static string _tradeQuery;

        internal static string _refund;

        internal static string _refundQuery;

        internal static string _subAppidWx;

        internal static string _subAppidMiniProgram;

        internal static string _reverseScan;

        public static void InitConfig(
            string privateKeyPem,
            string publicKeyPem,
            string orgId,
            string merchantInfoQuery,
            string jsapiScan,
            string tradeQuery,
            string refund,
            string refundQuery,
            string subAppidWx,
            string subAppidMiniProgram,
            string reverseScan)
        {
            _privateKeyPem = privateKeyPem;
            _publicKeyPem = publicKeyPem;
            _orgId = orgId;
            _merchantInfoQuery = merchantInfoQuery;
            _jsapiScan = jsapiScan;
            _tradeQuery = tradeQuery;
            _refund = refund;
            _refundQuery = refundQuery;
            _subAppidWx = subAppidWx;
            _subAppidMiniProgram = subAppidMiniProgram;
            _reverseScan = reverseScan;
        }
    }
}
