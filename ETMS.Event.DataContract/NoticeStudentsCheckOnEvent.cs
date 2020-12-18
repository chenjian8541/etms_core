using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsCheckOnEvent : Event
    {
        public NoticeStudentsCheckOnEvent(int tenantId) : base(tenantId)
        { }

        public long StudentCheckOnLogId { get; set; }
    }
}
