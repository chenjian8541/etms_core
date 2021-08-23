using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryPayrollUserGetOutput
    {
        public TeacherSalaryPayrollUserBaseInfo BaseInfo { get; set; }

        public List<TeacherSalaryPayrollUserFixedSalary> FixedSalarys { get; set; }

        public List<UserPerformances> PerformanceSalarys { get; set; }
    }

    public class TeacherSalaryPayrollUserBaseInfo
    {
        public long TeacherSalaryPayrollIdId { get; set; }

        public long PayrollUserId { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string DateDesc { get; set; }

        public byte Status { get; set; }

        public decimal PayItemSum { get; set; }

        public bool IsOpenClassPerformance { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryStatisticalRuleType"/>
        /// </summary>
        public byte StatisticalRuleType { get; set; }

        public string StatisticalRuleTypeDesc { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeType"/>
        /// </summary>
        public byte ComputeType { get; set; }

        public string ComputeTypeDesc { get; set; }

        public string ComputeTypeDescTag { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryGradientCalculateType"/>
        /// </summary>
        public byte GradientCalculateType { get; set; }

        public string GradientCalculateTypeDesc { get; set; }

        public string PerformanceSetDesc { get; set; }

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

    public class TeacherSalaryPayrollUserFixedSalary
    {
        public long Id { get; set; }

        public long FundsItemsId { get; set; }

        public string FundsItemsName { get; set; }

        public decimal AmountSum { get; set; }

        public byte FundsItemsType { get; set; }
    }
}
