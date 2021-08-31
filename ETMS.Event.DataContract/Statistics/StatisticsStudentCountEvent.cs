using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsStudentCountEvent : Event
    {
        public StatisticsStudentCountEvent(int tenantId) : base(tenantId)
        { }

        public DateTime Time { get; set; }
    }
}
