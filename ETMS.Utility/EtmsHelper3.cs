using ETMS.Utility.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Utility
{
    public class EtmsHelper3
    {
        public static int GetCent(decimal money)
        {
            return Convert.ToInt32(money * 100);
        }

        public static bool IsEffectiveDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return false;
            }
            if (DateTime.TryParse(date, out var tempDate))
            {
                return tempDate.IsEffectiveDate();
            }
            return false;
        }

        public static string OpenLinkGetIdEncrypt(long id)
        {
            var strEncrypt = $"8104{id}";
            var bytes = Encoding.UTF8.GetBytes(strEncrypt);
            var s = Convert.ToBase64String(bytes);
            return System.Web.HttpUtility.UrlEncode(s);
        }

        public static long OpenLinkGetIdDecrypt(string s)
        {
            var strEncrypt = System.Web.HttpUtility.UrlDecode(s);
            var bytes = Convert.FromBase64String(strEncrypt);
            var strCode = Encoding.UTF8.GetString(bytes);
            strCode = strCode.Substring(4);
            return strCode.ToLong();
        }

        public static string OpenLinkGetVtNo(int tenantId, long userId)
        {
            var timestamp = DateTime.Now.AddDays(1).EtmsGetTimestamp();
            var strEncrypt = $"{tenantId}_{userId}_{timestamp}";
            var bytes = Encoding.UTF8.GetBytes(strEncrypt);
            var s = Convert.ToBase64String(bytes);
            return System.Web.HttpUtility.UrlEncode(s);
        }

        public static OpenLinkAnalyzeView OpenLinkAnalyzeVtNo(string s)
        {
            var vtNo = System.Web.HttpUtility.UrlDecode(s);
            var bytes = Convert.FromBase64String(vtNo);
            var strCode = Encoding.UTF8.GetString(bytes);
            var myData = strCode.Split('_');
            return new OpenLinkAnalyzeView()
            {
                TenantId = myData[0].ToInt(),
                UserId = myData[1].ToLong(),
                ExTime = DataTimeExtensions.StampToDateTime(myData[2])
            };
        }
    }
}
