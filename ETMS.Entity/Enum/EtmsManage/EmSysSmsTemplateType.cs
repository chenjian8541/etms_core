using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysSmsTemplateType
    {
        /// <summary>
        /// 学员上课提醒(上课前一天)
        /// </summary>
        public const int NoticeStudentsOfClassBeforeDay = 0;

        /// <summary>
        /// 学员上课提醒(上课当天)
        /// </summary>
        public const int NoticeStudentsOfClassToday = 1;

        /// <summary>
        /// 上课点名通知
        /// </summary>
        public const int ClassCheckSign = 2;

        /// <summary>
        /// 请假申请审核结果通知
        /// </summary>
        public const int StudentLeaveApply = 3;

        /// <summary>
        /// 订单购买通知
        /// </summary>
        public const int StudentContracts = 4;

        /// <summary>
        /// 学员课时不足续费提醒
        /// </summary>
        public const int StudentCourseNotEnough = 5;

        /// <summary>
        /// 学员考勤通知(签到)
        /// </summary>
        public const int StudentCheckOnLogCheckIn = 6;

        /// <summary>
        /// 学员考勤通知(签退)
        /// </summary>
        public const int StudentCheckOnLogCheckOut = 7;

        /// <summary>
        /// 充值账户变动通知
        /// </summary>
        public const int StudentAccountRechargeChanged = 8;

        /// <summary>
        ///学员剩余课时(课时变动提醒)
        /// </summary>
        public const int StudentCourseSurplus = 9;

        /// <summary>
        /// 老师上课提醒 (30以后的为老师)
        /// </summary>
        public const int NoticeUserOfClassToday = 31;

        public static string GetSysSmsTemplateTypeDesc(int type)
        {
            switch (type)
            {
                case NoticeStudentsOfClassBeforeDay:
                    return "学员上课提醒(上课前一天)";
                case NoticeStudentsOfClassToday:
                    return "学员上课提醒(上课当天)";
                case ClassCheckSign:
                    return "上课点名通知";
                case StudentLeaveApply:
                    return "请假申请审核结果通知";
                case StudentContracts:
                    return "订单购买通知";
                case StudentCourseNotEnough:
                    return "学员课时不足续费提醒";
                case StudentCheckOnLogCheckIn:
                    return "学员考勤通知(签到)";
                case StudentCheckOnLogCheckOut:
                    return "学员考勤通知(签退)";
                case StudentAccountRechargeChanged:
                    return "充值账户变动通知";
                case StudentCourseSurplus:
                    return "学员课时变动提醒";
                case NoticeUserOfClassToday:
                    return "老师上课提醒";
            }
            return string.Empty;
        }
    }
}
