using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmTeacherSalaryGradientCalculateType
    {
        /// <summary>
        /// 无梯度
        /// </summary>
        public const byte None = 0;

        /// <summary>
        /// 超额累计
        /// </summary>
        public const byte MoreThanValue = 1;

        /// <summary>
        /// 全额累计
        /// </summary>
        public const byte AllValue = 2;
    }
}
