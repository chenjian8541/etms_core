using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Utility
{
    public static class DataTimeExtensions
    {
        public static string EtmsToString(this DateTime @this)
        {
            return @this.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string EtmsToDateString(this DateTime @this)
        {
            return @this.ToString("yyyy-MM-dd");
        }

        public static string EtmsToDateShortString(this DateTime @this)
        {
            return @this.ToString("MM/dd");
        }

        public static string EtmsToString(this DateTime? @this)
        {
            if (@this == null)
            {
                return string.Empty;
            }
            return @this.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string EtmsToMinuteString(this DateTime @this)
        {
            return @this.ToString("yyyy-MM-dd HH:mm");
        }

        public static string EtmsToOnlyMinuteString(this DateTime @this)
        {
            return @this.ToString("HH:mm");
        }

        public static string EtmsToMinuteString(this DateTime? @this)
        {
            if (@this == null)
            {
                return string.Empty;
            }
            return @this.Value.ToString("yyyy-MM-dd HH:mm");
        }

        public static string EtmsToOnlyMinuteString(this DateTime? @this)
        {
            if (@this == null)
            {
                return string.Empty;
            }
            return @this.Value.ToString("HH:mm");
        }

        public static string EtmsToDateString(this DateTime? @this)
        {
            if (@this == null)
            {
                return string.Empty;
            }
            return @this.Value.ToString("yyyy-MM-dd");
        }

        public static string EtmsToDateString2(this DateTime? @this)
        {
            if (@this == null)
            {
                return string.Empty;
            }
            return @this.Value.ToString("yyyy.MM.dd");
        }

        public static string EtmsToDateString3(this DateTime @this)
        {
            return @this.ToString("yyyyMMdd");
        }

        public static Tuple<int, int> EtmsGetAge(this DateTime? @this)
        {
            if (@this == null)
            {
                return null;
            }
            return @this.Value.EtmsGetAge();
        }

        public static Tuple<int, int> EtmsGetAge(this DateTime @this)
        {
            var birthday = @this;
            var now = DateTime.Now;
            var ageYear = now.Year - birthday.Year;
            if (now.Month < birthday.Month)
            {
                ageYear--;
            }
            else if (now.Month == birthday.Month && now.Day < birthday.Day)
            {
                ageYear--;
            }

            var ageMonth = 0;
            if (now.Month > birthday.Month)
            {
                ageMonth = now.Month - birthday.Month;
            }
            else if (now.Month < birthday.Month)
            {
                ageMonth = 12 - birthday.Month + now.Month;
            }
            else if (now.Month == birthday.Month)
            {
                if (now.Day < birthday.Day)
                {
                    ageMonth = 11;
                }
            }

            return Tuple.Create(ageYear, ageMonth);
        }

        public static long EtmsGetTimestamp(this DateTime @this)
        {
            var ts = @this - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)ts.TotalSeconds;
        }

        public static DateTime StampToDateTime(string timeStamp)
        {
            var dateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var lTime = long.Parse(timeStamp + "0000000");
            var toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }

        /// <summary>
        /// 判断是否为有效的时间
        /// 最小日期1949-01-01
        /// 最大日期3000-01-01
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsEffectiveDate(this DateTime @this)
        {
            var minDate = new DateTime(1949, 1, 1);
            var maxDate = new DateTime(3000, 1, 1);
            if (@this < minDate)
            {
                return false;
            }
            if (@this > maxDate)
            {
                return false;
            }
            return true;
        }

        public static DateTime ToBeijingTime(this DateTime @this)
        {
            return System.TimeZoneInfo.ConvertTimeFromUtc(@this, TimeZoneInfo.Local);
        }

        public static bool IsToday(this DateTime @this)
        {
            var now = DateTime.Now;
            return @this.Date == now.Date;
        }
    }
}
