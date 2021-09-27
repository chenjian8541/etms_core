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
        /// 未开通
        /// </summary>
        public const int NotApplied = 0;

        /// <summary>
        /// 审核通过，但未签署电子协议
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

        /// <summary>
        /// 审核通过且已签署电子协议
        /// </summary>
        public const int AuditPassAndSigned = 5;

        public static bool IsSuccess(int type)
        {
            return type == Passed || type == AuditPassAndSigned;
        }
    }
}
