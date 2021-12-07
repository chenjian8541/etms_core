using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Com.Fubei.OpenApi.Sdk.Enums;

namespace Com.Fubei.OpenApi.Sdk.Utils
{
    public class BarcodeDetector
    {
        /// <summary>
        /// 条形码规则
        /// </summary>
        private readonly SortedDictionary<EBarcodeType, Regex> _barcodeRules = new SortedDictionary<EBarcodeType, Regex>();

        private BarcodeDetector()
        {
            Init();
        }

        public static readonly BarcodeDetector Instance = new BarcodeDetector();

        public void Init()
        {
            // 默认规则
            // 支付宝
            AddOrUpdateBarcodeRule(EBarcodeType.Alipay, @"^((2[5-9]|30)\d{14,22})$");
            // 微信
            AddOrUpdateBarcodeRule(EBarcodeType.Wechat, @"^(1[0-5]\d{16})$");
            // 银联
            AddOrUpdateBarcodeRule(EBarcodeType.UnionPay, @"^(62[1-8]\d{16})$");
            // 翼支付
            //AddOrUpdateBarcodeRule(EBarcodeType.BestPayPayment, @"^(51\d{16})$");

        }

        /// <summary>
        /// 添加条码规则
        /// </summary>
        /// <param name="barcodeType"></param>
        /// <param name="pattern"></param>
        public void AddOrUpdateBarcodeRule(EBarcodeType barcodeType, string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            AddOrUpdateBarcodeRule(barcodeType, regex);
        }

        public void AddOrUpdateBarcodeRule(EBarcodeType barcodeType, Regex regex)
        {
            _barcodeRules[barcodeType] = regex;
        }

        public EBarcodeType GetBarcodeType(string code)
        {
            var barcodeType = EBarcodeType.Undetermined;
            foreach (var rule in _barcodeRules)
            {
                if (rule.Value.IsMatch(code))
                {
                    barcodeType = rule.Key;
                    break;
                }
            }
            return barcodeType;
        }
    }
}
