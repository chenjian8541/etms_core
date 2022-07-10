using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ETMS.Utility
{
    public class EtmsHelper2
    {
        public static Tuple<DateTime, DateTime> GetThisWeek(DateTime now)
        {
            now = now.Date;
            DateTime firstDate = now;
            switch (now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    firstDate = now;
                    break;
                case DayOfWeek.Tuesday:
                    firstDate = now.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    firstDate = now.AddDays(-2);
                    break;
                case DayOfWeek.Thursday:
                    firstDate = now.AddDays(-3);
                    break;
                case DayOfWeek.Friday:
                    firstDate = now.AddDays(-4);
                    break;
                case DayOfWeek.Saturday:
                    firstDate = now.AddDays(-5);
                    break;
                case DayOfWeek.Sunday:
                    firstDate = now.AddDays(-6);
                    break;
            }
            var endDate = firstDate.AddDays(6);
            return Tuple.Create(firstDate, endDate);
        }

        public static Tuple<DateTime, DateTime> GetThisMonth(DateTime now)
        {
            var monthDay = DateTime.DaysInMonth(now.Year, now.Month);
            var d1 = new DateTime(now.Year, now.Month, 1);
            var d2 = new DateTime(now.Year, now.Month, monthDay);
            return Tuple.Create(d1, d2);
        }

        public static Tuple<DateTime, DateTime> GetLastWeek(DateTime now)
        {
            var lastWeekDay = now.AddDays(-7);
            return GetThisWeek(lastWeekDay);
        }

        public static Tuple<DateTime, DateTime> GetLastMonth(DateTime now)
        {
            var lastMonthDay = now.AddMonths(-1);
            return GetThisMonth(lastMonthDay);
        }

        public static Tuple<DateTime, DateTime> GetThisYear(DateTime now)
        {
            var firstDate = new DateTime(now.Year, 1, 1);
            var endDate = new DateTime(now.Year, 12, 31);
            return Tuple.Create(firstDate, endDate);
        }

        public static List<DateTime> GetStartStepToAnd(DateTime start, DateTime end)
        {
            start = start.Date;
            end = end.Date;
            if (start == end)
            {
                return new List<DateTime>() { start };
            }
            if (start > end)
            {
                return new List<DateTime>();
            }
            var result = new List<DateTime>();
            while (start <= end)
            {
                result.Add(start);
                start = start.AddDays(1);
            }
            return result;
        }

        public static string GetPhoneEncrypt(string phone)
        {
            var strEncrypt = $"8104{phone}";
            var bytes = Encoding.UTF8.GetBytes(strEncrypt);
            return Convert.ToBase64String(bytes);
        }

        public static string GetPhoneDecrypt(string strEncrypt)
        {
            var bytes = Convert.FromBase64String(strEncrypt);
            var strCode = Encoding.UTF8.GetString(bytes);
            strCode = strCode.Substring(4);
            return strCode;
        }

        /// <summary>
        /// 加密tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static string GetTenantEncrypt(int tenantId)
        {
            var strEncrypt = $"8104{tenantId}";
            var bytes = Encoding.UTF8.GetBytes(strEncrypt);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 解密TenantNo
        /// 从第四位开始才有效
        /// </summary>
        /// <param name="strEncrypt"></param>
        /// <returns></returns>
        public static int GetTenantDecrypt(string strEncrypt)
        {
            if (strEncrypt.Equals("000"))
            {
                return 0;
            }
            var bytes = Convert.FromBase64String(strEncrypt);
            var strCode = Encoding.UTF8.GetString(bytes);
            strCode = strCode.Substring(4);
            return strCode.ToInt();
        }

        public static int GetTenantDecrypt2(string strEncrypt)
        {
            try
            {
                return GetTenantDecrypt(strEncrypt);
            }
            catch (Exception ex)
            {
                LOG.Log.Error($"解析机构编码错误：{strEncrypt}", ex, typeof(EtmsHelper2));
                return 0;
            }
        }

        public static string GetIdEncrypt(long id)
        {
            var strEncrypt = $"8104{id}";
            var bytes = Encoding.UTF8.GetBytes(strEncrypt);
            return Convert.ToBase64String(bytes);
        }

        public static int GetIdDecrypt(string strEncrypt)
        {
            if (strEncrypt.Equals("000"))
            {
                return 0;
            }
            var bytes = Convert.FromBase64String(strEncrypt);
            var strCode = Encoding.UTF8.GetString(bytes);
            strCode = strCode.Substring(4);
            return strCode.ToInt();
        }

        public static long GetIdDecrypt2(string strEncrypt)
        {
            if (strEncrypt.Equals("000"))
            {
                return 0;
            }
            var bytes = Convert.FromBase64String(strEncrypt);
            var strCode = Encoding.UTF8.GetString(bytes);
            strCode = strCode.Substring(4);
            return strCode.ToLong();
        }

        public static bool IsThisMonth(DateTime time)
        {
            var now = DateTime.Now;
            return time.Year == now.Year && time.Month == now.Month;
        }

        private const string OpenApi99Encrypt3DESKey = "etms_openapi_999";

        public static string GetEncryptOpenApi99(string str)
        {
            return CryptogramHelper.Encrypt3DES(str, OpenApi99Encrypt3DESKey);
        }

        public static string GetDecryptOpenApi99(string strNo)
        {
            return CryptogramHelper.Decrypt3DES(strNo, OpenApi99Encrypt3DESKey);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static string GetTenantEncryptOpenApi99(int tenantId)
        {
            var strEncrypt = $"8104{tenantId}";
            var bytes = Encoding.UTF8.GetBytes(strEncrypt);
            var baseStr = Convert.ToBase64String(bytes);
            return GetEncryptOpenApi99(baseStr);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="strNo"></param>
        /// <returns></returns>
        public static int GetTenantDecryptOpenApi99(string strNo)
        {
            var strEncrypt = GetDecryptOpenApi99(strNo);
            var bytes = Convert.FromBase64String(strEncrypt);
            var strCode = Encoding.UTF8.GetString(bytes);
            strCode = strCode.Substring(4);
            return strCode.ToInt();
        }

        public static string GetMediasKeys(List<string> keys)
        {
            if (keys == null || keys.Count == 0)
            {
                return string.Empty;
            }
            return string.Join('|', keys);
        }

        public static List<string> GetMediasUrl(string keys)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(keys))
            {
                return result;
            }
            var myMedias = keys.Split('|');
            foreach (var p in myMedias)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    result.Add(AliyunOssUtil.GetAccessUrlHttps(p));
                }
            }
            return result;
        }

        public static List<string> GetMediasUrl2(string keys)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(keys))
            {
                return result;
            }
            var myMedias = keys.Split('|');
            foreach (var p in myMedias)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    result.Add(p);
                }
            }
            return result;
        }

        public static string GetUrlEncrypt(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var baseStr = Convert.ToBase64String(bytes);
            return HttpUtility.UrlEncode(baseStr);
        }

        public static string GetUrlDecrypt(string strEncrypt)
        {
            var urlDecode = HttpUtility.UrlDecode(strEncrypt);
            var bytes = Convert.FromBase64String(urlDecode);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 获取endTime与当前时间差（时、分、秒）
        /// </summary>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static Tuple<int, int, int> GetCountDown(DateTime endTime)
        {
            var now = DateTime.Now.AddSeconds(-15);
            if (endTime < now)
            {
                return Tuple.Create(0, 0, 0);
            }
            var diff = endTime - now;
            return Tuple.Create(diff.Days*24+diff.Hours, diff.Minutes, diff.Seconds);
        }
    }
}
