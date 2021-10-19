using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 系统配置类型
    /// </summary>
    public struct EmAppConfigType
    {
        /// <summary>
        /// 机构配置
        /// </summary>
        public const byte TenantConfig = 0;

        /// <summary>
        /// 充值规则设置
        /// </summary>
        public const byte RechargeRuleConfig = 1;

        /// <summary>
        /// 约课规则设置
        /// </summary>
        public const byte ClassReservationSetting = 2;

        /// <summary>
        /// [老师工资]工资条项目(默认)
        /// </summary>
        public const byte TeacherSalaryDefaultFundsItems = 3;

        /// <summary>
        /// [老师工资]绩效工资统计规则
        /// </summary>
        public const byte TeacherSalaryGlobalRuleSetting = 4;

        /// <summary>
        /// 机构配置2
        /// </summary>
        public const byte TenantConfig2 = 5;
    }
}
