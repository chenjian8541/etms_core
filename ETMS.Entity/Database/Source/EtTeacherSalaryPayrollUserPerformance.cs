using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSalaryPayrollUserPerformance")]
    public class EtTeacherSalaryPayrollUserPerformance : Entity<long>
    {
        public long UserId { get; set; }

        public long TeacherSalaryPayrollId { get; set; }

        public long TeacherSalaryPayrollUserId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeType"/>
        /// </summary>
        public byte ComputeType { get; set; }

        public long RelationId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeMode"/>
        /// </summary>
        public byte ComputeMode { get; set; }

        public string ComputeDesc { get; set; }

        public decimal ComputeRelationValue { get; set; }

        public decimal ComputeSum { get; set; }

        public decimal SubmitSum { get; set; }
    }
}
