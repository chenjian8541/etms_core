using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class UserPerformances
    {
        public long CId { get; set; }

        public long TeacherSalaryPayrollId { get; set; }

        public long TeacherSalaryPayrollUserId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeType"/>
        /// </summary>
        public byte ComputeType { get; set; }

        public long RelationId { get; set; }

        public string RelationDesc { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeMode"/>
        /// </summary>
        public byte ComputeMode { get; set; }

        public string ComputeModeDesc { get; set; }

        public string ComputeDesc { get; set; }

        public decimal ComputeRelationValue { get; set; }

        public decimal ComputeSum { get; set; }

        public decimal SubmitSum { get; set; }
    }
}
