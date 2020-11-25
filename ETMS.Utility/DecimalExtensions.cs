﻿using System;
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
    }
}