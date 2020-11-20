using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfStudentEvaluateEvent : Event
    {
        public NoticeStudentsOfStudentEvaluateEvent(int tenantId) : base(tenantId)
        { }

        public long ClassRecordStudentId { get; set; }
    }
}
