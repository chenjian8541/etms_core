using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStatisticsEducationStudentMonth")]
    public class EtStatisticsEducationStudentMonth : Entity<long>
    {
        public long StudentId { get; set; }

        public DateTime Ot { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public decimal TeacherTotalClassTimes { get; set; }

        public int TeacherTotalClassCount { get; set; }

        public decimal TotalDeSum { get; set; }

        public int BeLateCount { get; set; }

        public int LeaveCount { get; set; }

        public int NotArrivedCount { get; set; }
    }
}
