using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ClassRemoveStudentEvent : Event
    {
        public ClassRemoveStudentEvent(int tenantId) : base(tenantId)
        {
        }

        public long StudentId { get; set; }

        public long CourseId { get; set; }
    }
}
