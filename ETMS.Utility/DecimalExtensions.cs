using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Utility
{
    public static class DecimalExtensions
    {
        public static string EtmsToString(this decimal @this)
        {
            if (@this % 1 == 0)
            {
                return @this.ToString("F0");
            }
            else
            {
                return @this.ToString("F1");
            }
        }

        public static string EtmsPercentage(this decimal @this)
        {
            if (@this <= 0)
            {
                return "0";
            }
            if (@this > 1)
            {
                return "100%";
            }
            return @this.ToString("P0");
        }

        public static string EtmsToString2(this decimal @this)
        {
            return @this.ToString("F2");
        }
    }
}
