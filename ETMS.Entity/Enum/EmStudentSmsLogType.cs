using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmStudentSmsLogType
    {
        /// <summary>
        /// 学员上课提醒
        /// </summary>
        public const int NoticeStudentsOfClassBeforeDay = 0;

        /// <summary>
        /// 学员上课提醒
        /// </summary>
        public const int NoticeStudentsOfClassToday = 1;

        /// <summary>
        /// 上课点名通知
        /// </summary>
        public const int NoticeClassCheckSign = 2;

        /// <summary>
        /// 请假申请审核结果通知
        /// </summary>
        public const int NoticeStudentLeaveApply = 3;

        /// <summary>
        /// 订单购买通知
        /// </summary>
        public const int NoticeStudentContracts = 4;
    }
}
