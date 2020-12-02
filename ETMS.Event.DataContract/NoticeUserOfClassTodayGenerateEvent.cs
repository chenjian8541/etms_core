using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeUserOfClassTodayGenerateEvent : Event
    {
        public NoticeUserOfClassTodayGenerateEvent(int tenantId) : base(tenantId)
        { }

        public DateTime ClassOt { get; set; }
    }
}