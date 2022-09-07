using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentToClassEvent : Event
    {
        public NoticeStudentToClassEvent(int tenantId) : base(tenantId)
        { }

        public List<long> StudentIds { get; set; }
    }
}
