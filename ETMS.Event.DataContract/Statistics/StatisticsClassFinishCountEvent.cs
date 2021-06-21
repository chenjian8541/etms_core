using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract.Statistics
{
    public class StatisticsClassFinishCountEvent : Event
    {
        public StatisticsClassFinishCountEvent(int tenantId) : base(tenantId)
        { }

        public long ClassId { get; set; }
    }
}

