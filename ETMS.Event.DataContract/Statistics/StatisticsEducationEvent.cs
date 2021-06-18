using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract.Statistics
{
    public class StatisticsEducationEvent: Event
    {
        public StatisticsEducationEvent(int tenantId) : base(tenantId)
        { }

        public DateTime Time { get; set; }
    }
}

