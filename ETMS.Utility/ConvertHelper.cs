using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ETMS.Utility
{
    /// <summary>
    /// 类型转换
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// 将string转换成int32
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static int ToInt(this object @this)
        {
            return Convert.ToInt32(@this);
        }

        /// <summary>
        /// 将string转换成long
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static long ToLong(this object @this)
        {
            return Convert.ToInt64(@this);
        }

        /// <summary>
        /// 将string转换成int?
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static int? ToIntNullable(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
            {
                return null;
            }
            return Convert.ToInt32(@this);
        }

        /// <summary>
        /// 转换成BigInteger类型
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static BigInteger ToBigInteger(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
            {
                return 0;
            }
            if (BigInteger.TryParse(@this, out BigInteger value))
            {
                return value;
            }
            return 0;
        }

        public static string ToSerializeObject<T>(this T @this) where T : class
        {
            return JsonConvert.SerializeObject(@this);
        }

        public static string ToDecimalDesc(this decimal @this)
        {
            var value = Convert.ToInt32(@this);
            if (@this == value)
            {
                return value.ToString();
            }
            return @this.ToString("F2");
        }

        public static decimal EtmsToRound(this decimal @this)
        {
            return decimal.Round(@this, 2);
        }

        public static int EtmsToPoints(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
            {
                return 0;
            }
            var temp = @this.ToInt();
            if (temp < 0)
            {
                return 0;
            }
            return temp;
        }
    }
}
