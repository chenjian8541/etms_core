using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentCourseDetailAnalyzeEvent : Event
    {
        public StudentCourseDetailAnalyzeEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public bool IsClassOfOneAutoOver { get; set; }
    }
}
