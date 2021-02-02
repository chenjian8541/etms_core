using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ClassOfOneAutoOverEvent : Event
    {
        public ClassOfOneAutoOverEvent(int tenantId) : base(tenantId)
        {
        }

        public long StudentId { get; set; }

        public long CourseId { get; set; }
    }
}
