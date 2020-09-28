using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员
    /// </summary>
    public struct EmStudentLeaveApplyHandleStatus
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

        /// <summary>
        /// 已撤销
        /// </summary>
        public const byte IsRevoke = 3;

        public static string GetStudentLeaveApplyHandleStatusDesc(byte t)
        {
            switch (t)
            {
                case EmStudentLeaveApplyHandleStatus.Unreviewed:
                    return "未审核";
                case EmStudentLeaveApplyHandleStatus.NotPass:
                    return "未通过";
                case EmStudentLeaveApplyHandleStatus.Pass:
                    return "已通过";
                case EmStudentLeaveApplyHandleStatus.IsRevoke:
                    return "已撤销";
            }
            return string.Empty;
        }

        public static string GetStudentLeaveApplyHandleStatusDescParent(byte t)
        {
            switch (t)
            {
                case EmStudentLeaveApplyHandleStatus.Unreviewed:
                    return "待处理";
                case EmStudentLeaveApplyHandleStatus.NotPass:
                    return "未通过";
                case EmStudentLeaveApplyHandleStatus.Pass:
                    return "已处理";
                case EmStudentLeaveApplyHandleStatus.IsRevoke:
                    return "已撤销";
            }
            return string.Empty;
        }
    }
}
