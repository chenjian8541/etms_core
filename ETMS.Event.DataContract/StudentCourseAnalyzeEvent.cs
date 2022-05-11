using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentCourseAnalyzeEvent : Event
    {
        public StudentCourseAnalyzeEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }

        public bool IsJobExecute { get; set; }
    }
}
