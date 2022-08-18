using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ReservationCourseSetGetOutput
    {
        public long CId { get; set; }

        public long CourseId { get; set; }

        public int LimitCount { get; set; }

        public string CourseName { get; set; }
    }
}
