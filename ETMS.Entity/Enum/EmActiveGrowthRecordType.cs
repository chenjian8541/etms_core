using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmActiveGrowthRecordType
    {
        /// <summary>
        /// 班级档案
        /// </summary>
        public const byte Class = 0;

        /// <summary>
        /// 学员档案
        /// </summary>
        public const byte Student = 1;

        public static string GetActiveGrowthRecordTypeDesc(byte t)
        {
            if (t == Class)
            {
                return "班级档案";
            }
            return "学员档案";
        }
    }
}
