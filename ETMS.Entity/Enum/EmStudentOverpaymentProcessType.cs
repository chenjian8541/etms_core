using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmStudentOverpaymentProcessType
    {
        /// <summary>
        /// 忽略不计
        /// </summary>
        public const byte Ignore = 0;

        /// <summary>
        /// 进入学员储值账户
        /// </summary>
        public const byte GoStudentAccountRecharge = 1;
    }
}
