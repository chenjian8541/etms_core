using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmTenantFubeiAccountApplyStatus
    {
        /// <summary>
        /// 未开通
        /// </summary>
        public const int NotApplied = 0;

        /// <summary>
        /// 审核通过
        /// </summary>
        public const int Passed = 1;

        /// <summary>
        /// 驳回
        /// </summary>
        public const int Fail = 2;

        /// <summary>
        /// 审核中
        /// </summary>
        public const int Applying = 3;
    }
}
