using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsCheckSignEvent : Event
    {
        public NoticeStudentsCheckSignEvent(int tenantId) : base(tenantId)
        { }

        public long ClassRecordId { get; set; }
    }
}
