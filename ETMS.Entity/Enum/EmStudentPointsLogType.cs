using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员积分变动类型
    /// </summary>
    public struct EmStudentPointsLogType
    {
        /// <summary>
        /// 报名/续费
        /// </summary>
        public const int StudentEnrolment = 0;

        /// <summary>
        /// 课堂奖励
        /// </summary>
        public const int ClassReward = 1;

        /// <summary>
        /// 礼品兑换
        /// </summary>
        public const int GiftExchange = 2;

        /// <summary>
        /// 撤销点名记录
        /// </summary>
        public const int ClassCheckSignRevoke = 3;

        /// <summary>
        /// 订单作废
        /// </summary>
        public const int OrderStudentEnrolmentRepeal = 4;

        /// <summary>
        /// 修改点名记录 返还积分
        /// </summary>
        public const int ModifyStudentClassRecordDe = 5;

        /// <summary>
        /// 修改点名记录 增加积分
        /// </summary>
        public const int ModifyStudentClassRecordAdd = 6;

        /// <summary>
        /// 考勤记上课
        /// </summary>
        public const int StudentCheckOn = 7;

        /// <summary>
        /// 撤销考勤记上课
        /// </summary>
        public const int StudentCheckOnRevoke = 8;

        /// <summary>
        /// 积分调整(增加)
        /// </summary>
        public const int StudentPointsAdjustAdd = 9;

        /// <summary>
        /// 积分调整(扣除)
        /// </summary>
        public const int StudentPointsAdjustDeduction = 10;

        /// <summary>
        /// 销售退单
        /// </summary>
        public const int OrderReturn = 11;

        /// <summary>
        /// 转课(增加)
        /// </summary>
        public const int TransferCourseAdd = 12;

        /// <summary>
        /// 转课(扣除)
        /// </summary>
        public const int TransferCourseDeduction = 13;

        /// <summary>
        /// 账户充值
        /// </summary>
        public const int StudentAccountRecharge = 14;

        /// <summary>
        /// 账户退款
        /// </summary>
        public const int StudentAccountRefund = 15;

        /// <summary>
        /// 推荐有奖(注册)
        /// </summary>
        public const int RecommendStudentRegistered = 16;

        /// <summary>
        /// 推荐有奖(报名)
        /// </summary>
        public const int RecommendStudentBuy = 17;

        public static string GetStudentPointsLogType(int type)
        {
            switch (type)
            {
                case StudentEnrolment:
                    return "报名/续费";
                case ClassReward:
                    return "课堂奖励";
                case GiftExchange:
                    return "礼品兑换";
                case ClassCheckSignRevoke:
                    return "撤销点名记录";
                case OrderStudentEnrolmentRepeal:
                    return "订单作废";
                case ModifyStudentClassRecordDe:
                    return "修改点名记录";
                case ModifyStudentClassRecordAdd:
                    return "修改点名记录";
                case StudentCheckOn:
                    return "考勤记上课";
                case StudentCheckOnRevoke:
                    return "撤销考勤记上课";
                case StudentPointsAdjustAdd:
                    return "积分调整(增加)";
                case StudentPointsAdjustDeduction:
                    return "积分调整(扣除)";
                case OrderReturn:
                    return "销售退单";
                case TransferCourseAdd:
                    return "转课(增加)";
                case TransferCourseDeduction:
                    return "转课(扣除)";
                case StudentAccountRecharge:
                    return "账户充值";
                case StudentAccountRefund:
                    return "账户退款";
                case RecommendStudentRegistered:
                    return "推荐有奖(注册)";
                case RecommendStudentBuy:
                    return "推荐有奖(报名/续费)";
            }
            return string.Empty;
        }

        public static string GetStudentPointsLogChangPointsDesc(int type, int point)
        {
            switch (type)
            {
                case StudentEnrolment:
                    return $"+{point}";
                case ClassReward:
                    return $"+{point}";
                case GiftExchange:
                    return $"-{point}";
                case ClassCheckSignRevoke:
                    return $"-{point}";
                case OrderStudentEnrolmentRepeal:
                    return $"-{point}";
                case ModifyStudentClassRecordDe:
                    return $"-{point}";
                case ModifyStudentClassRecordAdd:
                    return $"+{point}";
                case StudentCheckOn:
                    return $"+{point}";
                case StudentCheckOnRevoke:
                    return $"-{point}";
                case StudentPointsAdjustAdd:
                    return $"+{point}";
                case StudentPointsAdjustDeduction:
                    return $"-{point}";
                case OrderReturn:
                    return $"-{point}";
                case TransferCourseAdd:
                    return $"+{point}";
                case TransferCourseDeduction:
                    return $"-{point}";
                case StudentAccountRecharge:
                    return $"+{point}";
                case StudentAccountRefund:
                    return $"-{point}";
                case RecommendStudentRegistered:
                    return $"+{point}";
                case RecommendStudentBuy:
                    return $"+{point}";
            }
            return string.Empty;
        }
    }
}
