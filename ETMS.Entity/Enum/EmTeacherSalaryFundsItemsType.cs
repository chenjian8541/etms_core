using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmTeacherSalaryFundsItemsType
    {
        public const byte Add = 0;

        public const byte Deduction = 1;

        public static string GetTeacherSalaryFundsItemsTypeDesc(byte type)
        {
            return type == Add ? "加钱" : "扣钱";
        }
    }
}
