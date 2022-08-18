using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtReservationCourseSet")]
    public class EtReservationCourseSet : Entity<long>
    {
        public long CourseId { get; set; }

        public int LimitCount { get; set; }
    }
}
