using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSalaryPayrollUser")]
    public class EtTeacherSalaryPayrollUser : Entity<long>
    {
        public long TeacherSalaryPayrollId { get; set; }

        public long UserId { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? PayDate { get; set; }

        public decimal PayItemSum { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryStatisticalRuleType"/>
        /// </summary>
        public byte StatisticalRuleType { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeType"/>
        /// </summary>
        public byte ComputeType { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryGradientCalculateType"/>
        /// </summary>
        public byte GradientCalculateType { get; set; }

        public string PerformanceSetDesc { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryPayrollStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public DateTime Ot { get; set; }

        public long OpUserId { get; set; }
    }
}
