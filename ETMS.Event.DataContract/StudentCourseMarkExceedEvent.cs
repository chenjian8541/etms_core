using ETMS.Entity.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class StudentCourseMarkExceedEvent : Event
    {
        public StudentCourseMarkExceedEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public bool IsDeMyCourse { get; set; }

        public DeStudentClassTimesResult DeClassTimesResult { get; set; }
    }
}
