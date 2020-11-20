using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfHomeworkExDateEvent : Event
    {
        public NoticeStudentsOfHomeworkExDateEvent(int tenantId) : base(tenantId)
        { }
    }
}

