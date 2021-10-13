using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Statistics
{
    public class StatisticsLcsPayEvent : Event
    {
        public StatisticsLcsPayEvent(int tenantId) : base(tenantId)
        { }

        public DateTime StatisticsDate { get; set; }
    }
}


