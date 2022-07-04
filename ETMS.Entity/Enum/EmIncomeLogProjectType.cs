using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Enum
{
    public struct EmIncomeLogProjectType
    {
        /// <summary>
        /// 报名/续费
        /// </summary>
        public const long StudentEnrolment = -1;

        /// <summary>
        /// 补交费用
        /// </summary>
        public const long StudentEnrolmentAddPay = -2;

        /// <summary>
        /// 销售退单
        /// </summary>
        public const long RetuenOrder = -3;

        /// <summary>
        /// 转课
        /// </summary>
        public const long TransferCourse = -4;

        /// <summary>
        /// 账户充值
        /// </summary>
        public const long StudentAccountRecharge = -5;

        /// <summary>
        /// 账户退款
        /// </summary>
        public const long StudentAccountRefund = -6;

        /// <summary>
        /// 员工工资
        /// </summary>
        public const long TeacherSalary = -7;

        /// <summary>
        /// 推荐学员奖励(注册)
        /// </summary>
        public const long StudentRecommendRewardRegister = -8;

        /// <summary>
        /// 推荐学员奖励（消费）
        /// </summary>
        public const long StudentRecommendRewardConsumer = -9;

        public static string GetIncomeLogProjectType(List<EtIncomeProjectType> etIncomeProjectTypes, long type)
        {
            if (type >= 0)
            {
                var customizeProjectType = etIncomeProjectTypes.FirstOrDefault(p => p.Id == type);
                if (customizeProjectType != null)
                {
                    return customizeProjectType.Name;
                }
            }
            switch (type)
            {
                case StudentEnrolment:
                    return "报名/续费";
                case StudentEnrolmentAddPay:
                    return "报名补缴";
                case RetuenOrder:
                    return "销售退单";
                case TransferCourse:
                    return "转课";
                case StudentAccountRecharge:
                    return "账户充值";
                case StudentAccountRefund:
                    return "账户退款";
                case TeacherSalary:
                    return "员工工资";
                case StudentRecommendRewardRegister:
                    return "推荐学员注册";
                case StudentRecommendRewardConsumer:
                    return "推荐学员消费";
            }
            return string.Empty;
        }

        public static string GetIncomeLogProjectType(long type)
        {
            switch (type)
            {
                case StudentEnrolment:
                    return "报名/续费";
                case StudentEnrolmentAddPay:
                    return "报名补缴";
                case RetuenOrder:
                    return "销售退单";
                case TransferCourse:
                    return "转课";
                case StudentAccountRecharge:
                    return "账户充值";
                case StudentAccountRefund:
                    return "账户退款";
                case TeacherSalary:
                    return "员工工资";
                case StudentRecommendRewardRegister:
                    return "推荐学员注册";
                case StudentRecommendRewardConsumer:
                    return "推荐学员消费";
            }
            return string.Empty;
        }

        public static bool IsCanRevoke(long type)
        {
            return type > 0;
        }
    }
}
