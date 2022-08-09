using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmTeacherSalaryComputeValueType
    {
        /// <summary>
        /// 比例计算
        /// </summary>
        public const byte Proportion = 0;

        /// <summary>
        /// 固定金额
        /// </summary>
        public const byte FixedAmount = 1;
    }
}
