using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 试听申请审核状态
    /// </summary>
    public struct EmTryCalssApplyHandleStatus
    {
        /// <summary>
        /// 未审核
        /// </summary>
        public const byte Unreviewed = 0;

        /// <summary>
        /// 未通过
        /// </summary>
        public const byte NotPass = 1;

        /// <summary>
        /// 已通过
        /// </summary>
        public const byte Pass = 2;

        public static string GetTryCalssApplyHandleStatusDesc(byte t)
        {
            switch (t)
            {
                case Unreviewed:
                    return "未审核";
                case NotPass:
                    return "未通过";
                case Pass:
                    return "已通过";
            }
            return string.Empty;
        }
    }
}
