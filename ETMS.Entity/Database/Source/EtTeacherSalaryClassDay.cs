using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSalaryClassDay")]
    public class EtTeacherSalaryClassDay : Entity<long>
    {
        public long TeacherId { get; set; }

        public long ClassId { get; set; }

        public DateTime Ot { get; set; }

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
    }
}
