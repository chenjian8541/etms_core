using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class TempStudentNeedCheckGenerateEvent : Event
    {
        public TempStudentNeedCheckGenerateEvent(int tenantId) : base(tenantId)
        { }

        public List<long> ClassTimesIds { get; set; }

        public DateTime ClassOt { get; set; }
    }
}
