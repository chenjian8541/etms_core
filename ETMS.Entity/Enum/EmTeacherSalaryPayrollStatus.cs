﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmTeacherSalaryPayrollStatus
    {
        /// <summary>
        /// 未确认
        /// </summary>
        public const byte NotSure = 0;

        /// <summary>
        /// 已作废
        /// </summary>
        public const byte Repeal = 1;

        /// <summary>
        /// 已确认
        /// </summary>
        public const byte IsOK = 2;
    }
}
