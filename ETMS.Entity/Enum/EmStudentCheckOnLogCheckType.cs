using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmStudentCheckOnLogCheckType
    {
        /// <summary>
        /// 签到
        /// </summary>
        public const byte CheckIn = 0;

        /// <summary>
        /// 签退
        /// </summary>
        public const byte CheckOut = 1;

        public static string GetStudentCheckOnLogCheckTypeDesc(byte b)
        {
            if (b == CheckIn)
            {
                return "签到";
            }
            return "签退";
        }
    }
}
