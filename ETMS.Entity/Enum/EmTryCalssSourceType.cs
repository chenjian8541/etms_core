using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员试听来源
    /// </summary>
    public struct EmTryCalssSourceType
    {
        /// <summary>
        /// 游客
        /// </summary>
        public const byte Tourists = 0;

        /// <summary>
        /// 学员
        /// </summary>
        public const byte Student = 1;

        public static string GetTryCalssSourceTypeDesc(byte b)
        {
            if (b == Tourists)
            {
                return "游客";
            }
            if (b == Student)
            {
                return "学员";
            }
            return string.Empty;
        }
    }
}
