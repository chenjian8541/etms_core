using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfGrowthRecordEvent : Event
    {
        public NoticeStudentsOfGrowthRecordEvent(int tenantId) : base(tenantId)
        { }

        public long GrowthRecordId { get; set; }
    }
}
