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
    }
}
