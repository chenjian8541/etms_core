using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class StudentCourseExTimeDeEvent : Event
    {
        public StudentCourseExTimeDeEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }

        public long CourseId { get; set; }
    }
}
