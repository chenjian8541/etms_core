using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStatisticsEducationClassMonth")]
    public class EtStatisticsEducationClassMonth : Entity<long>
    {
        public long ClassId { get; set; }

        public DateTime Ot { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public decimal TeacherTotalClassTimes { get; set; }

        public int TeacherTotalClassCount { get; set; }

        public decimal TotalDeSum { get; set; }

        public int AttendNumber { get; set; }

        public int NeedAttendNumber { get; set; }

        public decimal Attendance { get; set; }
    }
}
