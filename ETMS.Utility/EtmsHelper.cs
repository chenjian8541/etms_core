using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace ETMS.Utility
{
    public class EtmsHelper
    {
        public static string PeopleDesc(string name, string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return name;
            }
            return $"{name}({phone})";
        }

        public static string PeopleSecrecy(string name, string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return name;
            }
            return $"{name}({Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2")})";
        }

        public static string GetWeekDesc(byte week)
        {
            switch (week)
            {
                case (int)DayOfWeek.Sunday:
                    return "日";
                case (int)DayOfWeek.Monday:
                    return "一";
                case (int)DayOfWeek.Tuesday:
                    return "二";
                case (int)DayOfWeek.Wednesday:
                    return "三";
                case (int)DayOfWeek.Thursday:
                    return "四";
                case (int)DayOfWeek.Friday:
                    return "五";
                default:
                    return "六";
            }
        }

        public static string GetTimeDesc(int time)
        {
            if (time <= 0)
            {
                return string.Empty;
            }
            var timeValue = time.ToString();
            if (timeValue.Length == 4)
            {
                return $"{timeValue.Substring(0, 2)}:{timeValue.Substring(2, 2)}";
            }
            if (timeValue.Length < 3)
            {
                if (timeValue.Length == 1)
                {
                    return $"00:0{timeValue}";
                }
                else
                {
                    return $"00:{timeValue}";
                }
            }
            return $"0{timeValue.Substring(0, 1)}:{timeValue.Substring(1, 2)}";
        }

        public static string GetTimeDesc(int startTime, int endTime, string tag = "~")
        {
            return $"{GetTimeDesc(startTime)}{tag}{GetTimeDesc(endTime)}";
        }

        public static string GetTimeDuration(int startTime, int endTime)
        {
            var startDate = new DateTime(2020, 01, 01, startTime / 100, startTime % 100, 0);
            var endDate = new DateTime(2020, 01, 01, endTime / 100, endTime % 100, 0);
            var times = endDate - startDate;
            var s = times.Hours + times.Minutes / 60.0;
            return s.ToString("F2");
        }
        public static string GetMuIds(IEnumerable<long> ids)
        {
            if (ids == null || !ids.Any())
            {
                return string.Empty;
            }
            return $",{string.Join(',', ids)},";
        }

        public static string GetMuIds(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return string.Empty;
            }
            return $",{string.Join(',', ids)},";
        }

        public static List<long> AnalyzeMuIds(string muIds)
        {
            var result = new List<long>();
            if (string.IsNullOrEmpty(muIds))
            {
                return result;
            }
            var ids = muIds.Split(',');
            foreach (var p in ids)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    result.Add(p.ToLong());
                }
            }
            return result;
        }

        public static int GetTotalPage(int rowCount, int pageSize)
        {
            var pageCount = rowCount / pageSize;
            if (rowCount % pageSize > 0)
            {
                pageCount++;
            }
            return pageCount;
        }

        public static Tuple<int, int> GetDffTime(DateTime startTime, DateTime endTime)
        {
            if (startTime > endTime)
            {
                return Tuple.Create(0, 0);
            }
            var dayDff = new DateDiff(startTime, endTime);
            var months = dayDff.Years * 12 + dayDff.Months;
            return Tuple.Create(months, dayDff.Days);
        }

        public static T DeepCopy<T>(T obj)
        {
            object retval;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        public static string EtmsSerializeObject<T>(T obj) where T : class
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T EtmsDeserializeObject<T>(string str) where T : class
        {
            return JsonConvert.DeserializeObject(str, typeof(T)) as T;
        }

        public static bool IsMobilePhone(string input)
        {
            var regex = new Regex("^1[3456789]\\d{9}$");
            return regex.IsMatch(input);
        }

        public static bool CheckIsDigitOrLetter(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }
            var pattern = @"^[a-zA-Z0-9]*$";
            if (Regex.IsMatch(str, pattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetOtFriendlyDesc(DateTime getTime)
        {
            var time = DateTime.Now - getTime;
            if (time.TotalDays > 7)
            {
                return getTime.EtmsToMinuteString();
            }
            if (time.TotalHours > 24)
            {
                return $"{Math.Floor(time.TotalDays)}天前";
            }
            if (time.TotalHours > 1)
            {
                return $"{Math.Floor(time.TotalHours)}小时前";
            }
            if (time.TotalMinutes > 1)
            {
                return $"{Math.Floor(time.TotalMinutes)}分钟前";
            }
            if (time.TotalSeconds == 0)
            {
                return "1秒前";
            }
            return $"{Math.Floor(time.TotalSeconds)}秒前";
        }

        /// <summary>
        /// 判断是否为白天(正常使用时间)
        /// 排查JOB 执行时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool CheckIsDaytime(DateTime time)
        {
            var hour = time.Hour;
            return hour > 6 && hour < 23;
        }

        public static string DescPrefix(string desc, string prefix, string intervalStr = "：")
        {
            if (string.IsNullOrEmpty(desc))
            {
                return string.Empty;
            }
            return $"{prefix}{intervalStr}{desc}";
        }

        public static List<string> GetStrList(string str, char intervalStr = ',')
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<string>();
            }
            return str.Split(intervalStr).ToList();
        }

        public static string GetMoneyChangeDesc(decimal cgValue)
        {
            if (cgValue < 0)
            {
                return cgValue.ToString("F2");
            }
            return $"+{cgValue.ToString("F2")}";
        }

        public static string GetChangeLogTimeDesc(DateTime time)
        {
            if (time.Year == DateTime.Now.Year)
            {
                return time.ToString("MM-dd HH:mm");
            }
            return time.ToString("yyyy-MM-dd HH:mm");
        }

        public static DateTime GetTime(DateTime ot, int time)
        {
            return new DateTime(ot.Year, ot.Month, ot.Day, time / 100, time % 100, 0);
        }

        public static int GetTimeHourAndMinuteDesc(DateTime time)
        {
            var temp = time.ToString("HHmm");
            return temp.ToInt();
        }
    }
}
