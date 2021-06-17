using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
