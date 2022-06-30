using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Utility;

namespace ETMS.Business.Common
{
    public class OrderNumberLib
    {

        public static string GiftExchangeOrderNumber()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(100, 999);
            return $"G{strTime}{strRandom}";
        }

        public static string EnrolmentOrderNumber()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(100, 999);
            return $"{strTime}{strRandom}";
        }

        public static string EnrolmentOrderNumber2()
        {
            var strTime = DateTime.Now.ToString("yyMMddHHmmss");
            var strRandom = new Random().Next(1000, 9999);
            return $"L{strTime}{strRandom}";
        }

        public static string GetReturnOrderNumber()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(10, 99);
            return $"T{strTime}{strRandom}";
        }

        public static string GetTransferCoursesOrderNumber()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(10, 99);
            return $"Z{strTime}{strRandom}";
        }

        public static string CouponsGenerateNo()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(100, 999);
            return $"{strTime}{strRandom}";
        }

        public static string StudentAccountRecharge()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(100, 999);
            return $"{strTime}{strRandom}";
        }

        public static string StudentAccountRefund()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(100, 999);
            return $"{strTime}{strRandom}";
        }

        /// <summary>
        /// 付呗退款单号
        /// 通过退款单号可以分析出机构id
        /// </summary>
        /// <returns></returns>
        public static string FubeiRefundOrder(int tenantId)
        {
            var strTime = DateTime.Now.ToString("yMdHms");
            var strRandom = new Random().Next(10, 99);
            return $"R{tenantId}_{strTime}{strRandom}";
        }

        public static int FubeiRefundOrderGetTenantId(string merchant_refund_sn)
        {
            var strS = merchant_refund_sn.Split('_');
            return strS[0].TrimStart('R').ToInt();
        }

        public static string SuixingPayOrder()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(100, 999);
            return $"{strTime}{strRandom}";
        }

        public static string SuixingRefundOrder()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(100, 999);
            return $"R{strTime}{strRandom}";
        }
    }
}
