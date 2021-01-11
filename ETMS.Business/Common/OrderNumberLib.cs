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

        public static string GetReturnOrderNumber()
        {
            var strTime = DateTime.Now.ToString("TyyyyMMddHHmms");
            var strRandom = new Random().Next(100, 999);
            return $"{strTime}{strRandom}";
        }

        public static string GetTransferCoursesOrderNumber()
        {
            var strTime = DateTime.Now.ToString("ZyyyyMMddHHmms");
            var strRandom = new Random().Next(100, 999);
            return $"{strTime}{strRandom}";
        }

        public static string CouponsGenerateNo()
        {
            var strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var strRandom = new Random().Next(100, 999);
            return $"{strTime}{strRandom}";
        }
    }
}
