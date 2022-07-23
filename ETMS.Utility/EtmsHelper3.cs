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

        public static string GetWeeksDesc(List<byte> weeks)
        {
            if (weeks == null || weeks.Count == 0)
            {
                return string.Empty;
            }
            if (weeks.Count == 7)
            {
                return "每天";
            }
            if (weeks.Count == 1)
            {
                return $"每周{EtmsHelper.GetWeekDesc(weeks[0])}";
            }
            var tempWeek = weeks.OrderBy(j => j);
            var str = new StringBuilder("每");
            foreach (var itemWeek in tempWeek)
            {
                str.Append($"周{EtmsHelper.GetWeekDesc(itemWeek)}、");
            }
            return str.ToString().TrimEnd('、');
        }

        public static DateTime GetTodayTime(int hourAndMinute)
        {
            var hour = hourAndMinute / 100;
            var minute = hourAndMinute % 100;
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
        }

        public static DateTime GetDateTime(DateTime myDate, int hourAndMinute)
        {
            var hour = hourAndMinute / 100;
            var minute = hourAndMinute % 100;
            return new DateTime(myDate.Year, myDate.Month, myDate.Day, hour, minute, 0);
        }

        /// <summary>
        /// 粗略计算消耗的短信条数
        /// </summary>
        /// <param name="smsContent"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int SmsCountCalculate(string smsContent, int count)
        {
            var smsLength = smsContent.Length;
            if (smsLength < 70)
            {
                return count;
            }
            var allCount = smsLength / 67;
            var exceedNumCount = allCount % 67 > 0 ? 1 : 0;
            var itemCount = allCount + exceedNumCount;
            return itemCount * count;
        }

        public static int GetBirthdayTag(DateTime birthday)
        {
            return birthday.ToString("MMdd").ToInt();
        }

        /// <summary>
        /// 获取GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
