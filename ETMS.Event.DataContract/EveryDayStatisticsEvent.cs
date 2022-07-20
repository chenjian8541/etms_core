using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class EveryDayStatisticsEvent: Event
    {
        public EveryDayStatisticsEvent(int tenantId) : base(tenantId)
        {
        }

        public DateTime Time { get; set; }
    }
}
