using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfClassTodayGenerateEvent : Event
    {
        public NoticeStudentsOfClassTodayGenerateEvent(int tenantId) : base(tenantId)
        { }

        public DateTime ClassOt { get; set; }
    }
}
