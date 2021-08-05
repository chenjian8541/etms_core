using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Enum;

namespace ETMS.Entity.View
{
    public class TeacherSalaryGlobalRuleView
    {
        /// <summary>
        /// 统计规则
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryStatisticalRuleType"/>
        /// </summary>
        public byte StatisticalRuleType { get; set; }

        /// <summary>
        /// 梯度计算
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryGradientCalculateType"/>
        /// </summary>
        public byte GradientCalculateType { get; set; }

        /// <summary>
        /// 补课计入到课人次
        /// <see cref="EmBool"/>
        /// </summary>
        public byte IncludeArrivedMakeUpStudent { get; set; }

        /// <summary>
        /// 试听计入到课人次
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IncludeArrivedTryCalssStudent { get; set; }
    }
}
