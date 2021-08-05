using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class TeacherSalaryPerformanceRuleView
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
    }
}
