using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing
{
    public static class Config
    {
        private static string _privateKeyPem;

        private static string _publicKeyPem;

        private static string _merchantInfoQuery;

        private static string _jsapiScan;

        private static string _tradeQuery;

        private static string _refund;

        private static string _refundQuery;

        public static void InitConfig(
            string privateKeyPem,
            string publicKeyPem,
            string merchantInfoQuery,
            string jsapiScan,
            string tradeQuery,
            string refund,
            string refundQuery)
        {
            _privateKeyPem = privateKeyPem;
            _publicKeyPem = publicKeyPem;
            _merchantInfoQuery = merchantInfoQuery;
            _jsapiScan = jsapiScan;
            _tradeQuery = tradeQuery;
            _refund = refund;
            _refundQuery = refundQuery;
        }
    }
}
