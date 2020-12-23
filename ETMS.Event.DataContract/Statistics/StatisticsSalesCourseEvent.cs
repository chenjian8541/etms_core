using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsSalesCourseEvent : Event
    {
        public StatisticsSalesCourseEvent(int tenantId) : base(tenantId)
        { }

        public DateTime StatisticsDate { get; set; }
    }
}

