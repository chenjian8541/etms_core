using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsSalesProductEvent : Event
    {
        public StatisticsSalesProductEvent(int tenantId) : base(tenantId)
        { }

        public DateTime StatisticsDate { get; set; }
    }
}
