using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmLcswApplyStatus
    {
        /// <summary>
        /// 未申请
        /// </summary>
        public const int NotApplied = 0;

        /// <summary>
        /// 申请中
        /// </summary>
        public const int Applying = 1;

        /// <summary>
        /// 审核通过
        /// </summary>
        public const int Passed = 2;

        /// <summary>
        /// 审核失败
        /// </summary>
        public const int Fail = 3;
    }
}
