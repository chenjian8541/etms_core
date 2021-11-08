using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryUserPerformanceDetailGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeType"/>
        /// </summary>
        public byte ComputeType { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeMode"/>
        /// </summary>
        public byte ComputeMode { get; set; }

        public string ClassName { get; set; }

        public string CourseName { get; set; }

        public long ClassRecordId { get; set; }

        public string ClassOtDesc { get; set; }

        public string ClassTimeDesc { get; set; }

        public string WeekDesc { get; set; }

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

        public decimal ComputeSum { get; set; }

        public decimal BascMoney { get; set; }
    }
}
