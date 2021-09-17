using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Utility
{
    public static class ApiAddress
    {
        private static string _checkname;

        private static string _addMerchant;

        private static string _updateMerchant;

        private static string _querMerchant;

        private static string _getSettlement;

        private static string _addTermina;

        private static string _queryTermina;

        private static string _barcodePay;

        private static string _unifiedOrder;

        private static string _query;

        private static string _refund;

        public static string Checkname
        {
            get
            {
                if (string.IsNullOrEmpty(_checkname))
                {
                    _checkname = $"{Config._apiMpHostMerchant}/merchant/200/checkname";
                }
                return _checkname;
            }
        }

        public static string AddMerchant
        {
            get
            {
                if (string.IsNullOrEmpty(_addMerchant))
                {
                    _addMerchant = $"{Config._apiMpHostMerchant}/merchant/200/add";
                }
                return _addMerchant;
            }
        }

        public static string UpdateMerchant
        {
            get
            {
                if (string.IsNullOrEmpty(_updateMerchant))
                {
                    _updateMerchant = $"{Config._apiMpHostMerchant}/merchant/200/update";
                }
                return _updateMerchant;
            }
        }

        public static string QuerMerchant
        {
            get
            {
                if (string.IsNullOrEmpty(_querMerchant))
                {
                    _querMerchant = $"{Config._apiMpHostMerchant}/merchant/200/query";
                }
                return _querMerchant;
            }
        }

        public static string GetSettlement
        {
            get
            {
                if (string.IsNullOrEmpty(_getSettlement))
                {
                    _getSettlement = $"{Config._apiMpHostMerchant}/merchant/withdraw/settlementrecords";
                }
                return _getSettlement;
            }
        }

        public static string AddTermina
        {
            get
            {
                if (string.IsNullOrEmpty(_addTermina))
                {
                    _addTermina = $"{Config._apiMpHostMerchant}/terminal/100/add";
                }
                return _addTermina;
            }
        }

        public static string QueryTermina
        {
            get
            {
                if (string.IsNullOrEmpty(_queryTermina))
                {
                    _queryTermina = $"{Config._apiMpHostMerchant}/terminal/100/query";
                }
                return _queryTermina;
            }
        }

        public static string BarcodePay
        {
            get
            {
                if (string.IsNullOrEmpty(_barcodePay))
                {
                    _barcodePay = $"{Config._apiMpHostPay}/pay/100/barcodepay";
                }
                return _barcodePay;
            }
        }

        public static string UnifiedOrder
        {
            get
            {
                if (string.IsNullOrEmpty(_unifiedOrder))
                {
                    _unifiedOrder = $"{Config._apiMpHostPay}/pay/100/jspay";
                }
                return _unifiedOrder;
            }
        }

        public static string Query
        {
            get
            {
                if (string.IsNullOrEmpty(_query))
                {
                    _query = $"{Config._apiMpHostPay}/pay/100/query";
                }
                return _query;
            }
        }

        public static string Refund
        {
            get
            {
                if (string.IsNullOrEmpty(_refund))
                {
                    _refund = $"{Config._apiMpHostPay}/pay/100/refund";
                }
                return _refund;
            }
        }
    }
}
