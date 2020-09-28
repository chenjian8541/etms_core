using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ConsumeStudentCourseEvent : Event
    {
        public ConsumeStudentCourseEvent(int tenantId, long studentCourseDetailId, DateTime deTime) : base(tenantId)
        {
            this.StudentCourseDetailId = studentCourseDetailId;
            this.DeTime = deTime;
        }

        public long StudentCourseDetailId { get; set; }

        public DateTime DeTime { get; set; }
    }
}
