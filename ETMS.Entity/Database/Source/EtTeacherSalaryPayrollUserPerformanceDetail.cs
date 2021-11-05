using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 工资绩效详情
    /// </summary>
    [Table("EtTeacherSalaryPayrollUserPerformanceDetail")]
    public class EtTeacherSalaryPayrollUserPerformanceDetail : Entity<long>
    {
        public long UserId { get; set; }

        public long TeacherSalaryPayrollId { get; set; }

        public long TeacherSalaryPayrollUserId { get; set; }

        public long UserPerformanceId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeType"/>
        /// </summary>
        public byte ComputeType { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeMode"/>
        /// </summary>
        public byte ComputeMode { get; set; }

        public long ClassId { get; set; }

        public long CourseId { get; set; }

        public long ClassRecordId { get; set; }

        public DateTime ClassOt { get; set; }

        public byte Week { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public decimal TeacherClassTimes { get; set; }

        public decimal StudentClassTimes { get; set; }

        public decimal DeSum { get; set; }

        public int ArrivedAndBeLateCount { get; set; }

        public int ArrivedCount { get; set; }

        public int BeLateCount { get; set; }

        public int LeaveCount { get; set; }

        public int NotArrivedCount { get; set; }

        public int TryCalssStudentCount { get; set; }

        public int MakeUpStudentCount { get; set; }

        public int TryCalssEffectiveCount { get; set; }

        public int MakeUpEffectiveCount { get; set; }

        public decimal BascMoney { get; set; }

        public decimal ComputeSum { get; set; }
    }
}
