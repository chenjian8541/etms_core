using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class StatisticsTeacherSalaryClassTimesEvent : Event
    {
        public StatisticsTeacherSalaryClassTimesEvent(int tenantId) : base(tenantId)
        {
        }

        public long ClassRecordId { get; set; }
    }
}
