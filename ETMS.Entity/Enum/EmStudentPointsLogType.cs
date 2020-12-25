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
            }
            return string.Empty;
        }
    }
}
