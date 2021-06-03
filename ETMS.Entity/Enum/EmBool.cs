using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmBool
    {
        /// <summary>
        /// false
        /// </summary>
        public const byte False = 0;

        /// <summary>
        /// True
        /// </summary>
        public const byte True = 1;

        public static string GetBoolDesc(byte t)
        {
            return t == True ? "是" : "否";
        }
    }
}
