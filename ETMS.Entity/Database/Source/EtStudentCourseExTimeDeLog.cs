using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStudentCourseExTimeDeLog")]
    public class EtStudentCourseExTimeDeLog : Entity<long>
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public long StudentCourseDetailId { get; set; }

        public DateTime CompDate { get; set; }

        public int ClassCount { get; set; }

        public int DeTimes { get; set; }
    }
}
