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

        public static string EtmsToMinuteString(this DateTime? @this)
        {
            if (@this == null)
            {
                return string.Empty;
            }
            return @this.Value.ToString("yyyy-MM-dd HH:mm");
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

        public static int? EtmsGetAge(this DateTime? @this)
        {
            if (@this == null)
            {
                return null;
            }
            var birthday = @this.Value;
            var now = DateTime.Now;
            var age = now.Year - birthday.Year;
            if (now.Month < birthday.Month)
            {
                age--;
            }
            else if (now.Month == birthday.Month && now.Day < birthday.Day)
            {
                age--;
            }
            return age;
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
    }
}
