using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSchooltimeConfig")]
    public class EtTeacherSchooltimeConfig : Entity<long>
    {
        public long TeacherId { get; set; }

        public long? CourseId { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public string Weeks { get; set; }

        public bool IsJumpHoliday { get; set; }

        public string RuleDesc { get; set; }

        public string TimeDesc { get; set; }
    }
}
