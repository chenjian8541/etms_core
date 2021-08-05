using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmTeacherSalaryStatisticalRuleType
    {
        /// <summary>
        /// 按合计值统计
        /// </summary>
        public const byte TotalClassTimesFirst = 0;

        /// <summary>
        /// 按每个课次单独统计
        /// </summary>
        public const byte AloneClassTimes = 1;
    }
}
