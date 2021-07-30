using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmStudentLeaveApplyMonthLimitType
    {
        public const byte Week = 0;

        public const byte Month = 1;

        public const byte Year = 2;

        public static string GetLimitTypeDesc(byte type)
        {
            switch (type)
            {
                case Week:
                    return "周";
                case Month:
                    return "月";
                case Year:
                    return "年";
            }
            return string.Empty;
        }
    }
}
