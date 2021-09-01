using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncTeacherMonthClassTimesEvent : Event
    {
        public SyncTeacherMonthClassTimesEvent(int tenantId) : base(tenantId)
        { }

        public List<long> TeacherIds { get; set; }

        public List<YearAndMonth> YearAndMonthList { get; set; }
    }
}
