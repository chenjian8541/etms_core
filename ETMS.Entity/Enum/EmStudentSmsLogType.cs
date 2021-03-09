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
        /// 请假审核结果通知
        /// </summary>
        public const int NoticeStudentLeaveApply = 3;

        /// <summary>
        /// 订单购买通知
        /// </summary>
        public const int NoticeStudentContracts = 4;

        /// <summary>
        /// 课时不足续费提醒
        /// </summary>
        public const int StudentCourseNotEnough = 5;

        /// <summary>
        /// 学员考勤
        /// </summary>
        public const int StudentCheckOnLog = 6;

        /// <summary>
        /// 充值帐户变动提醒
        /// </summary>
        public const int StudentAccountRechargeChanged = 7;

        public static string GetStudentSmsLogTypeDesc(int t)
        {
            switch (t)
            {
                case NoticeStudentsOfClassBeforeDay:
                    return "学员上课提醒";
                case NoticeStudentsOfClassToday:
                    return "学员上课提醒";
                case NoticeClassCheckSign:
                    return "上课点名通知";
                case NoticeStudentLeaveApply:
                    return "请假审核通知";
                case NoticeStudentContracts:
                    return "订单购买通知";
                case StudentCourseNotEnough:
                    return "课时不足续费提醒";
                case StudentCheckOnLog:
                    return "学员考勤";
                case StudentAccountRechargeChanged:
                    return "充值帐户变动提醒";
            }
            return string.Empty;
        }
    }
}
