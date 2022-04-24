using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSchooltimeConfigDetail")]
    public class EtTeacherSchooltimeConfigDetail : Entity<long>
    {
        public long SchooltimeConfigId { get; set; }

        public long TeacherId { get; set; }

        public long? CourseId { get; set; }

        public byte Week { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public byte IsJumpHoliday { get; set; }
    }
}
