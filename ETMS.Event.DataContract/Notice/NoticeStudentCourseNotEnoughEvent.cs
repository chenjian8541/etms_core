using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentCourseNotEnoughEvent : Event
    {
        public NoticeStudentCourseNotEnoughEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }

        public long CourseId { get; set; }
    }
}

