using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Statistics
{
    public class StatisticsStudentCheckEvent : Event
    {
        public StatisticsStudentCheckEvent(int tenantId) : base(tenantId)
        { }

        public DateTime CheckOt { get; set; }
    }
}
