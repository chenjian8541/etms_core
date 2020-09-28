using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 性别
    /// </summary>
    public struct EmGender
    {
        /// <summary>
        /// 男
        /// </summary>
        public const int Man = 0;

        /// <summary>
        /// 女
        /// </summary>
        public const int Woman = 1;

        public static string GetGenderDesc(byte? gender)
        {
            if (gender == null)
            {
                return string.Empty;
            }
            return gender == EmGender.Man ? "男" : "女";
        }
    }
}
