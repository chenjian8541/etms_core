using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 上课积分奖励申请状态
    /// </summary>
    public struct EmClassRecordPointsApplyHandleStatus
    {
        /// <summary>
        /// 未审核
        /// </summary>
        public const byte NotCheckd = 0;

        /// <summary>
        /// 未通过
        /// </summary>
        public const byte NotCheckPass = 1;

        /// <summary>
        /// 已通过
        /// </summary>
        public const byte CheckPass = 2;


        public static string GetClassRecordPointsApplyHandleStatusDesc(byte b)
        {
            switch (b)
            {
                case NotCheckd:
                    return "未审核";
                case NotCheckPass:
                    return "未通过";
                case CheckPass:
                    return "已通过";
            }
            return string.Empty;
        }
    }
}
