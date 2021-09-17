using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw
{
    public static class Config
    {
        public const int TIME_OUT = 10000;

        /// <summary>
        /// 扫呗支付接口
        /// 扫呗服务器地址 测试地址-:http://test.lcsw.cn:8022/lcsw
        /// 测试地址二：http://test.lcsw.cn:8033/lcs
        /// 测试地址三：http://test.lcsw.cn:8045/lcsw
        /// 正式版地址：https://pay.lcsw.cn/lcsw
        /// </summary>
        public static string _apiMpHostPay = "";

        /// <summary>
        /// 扫呗商户接口
        /// </summary>
        public static string _apiMpHostMerchant = "";


        public static string _instNo = "";

        public static string _instToken = "";

        public static void InitConfig(string apiMpHostPay, string apiMpHostMerchant, string instNo, string instToken)
        {
            _apiMpHostPay = apiMpHostPay;
            _apiMpHostMerchant = apiMpHostMerchant;
            _instNo = instNo;
            _instToken = instToken;
        }
    }
}
