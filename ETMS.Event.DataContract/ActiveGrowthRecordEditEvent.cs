using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ActiveGrowthRecordEditEvent : Event
    {
        public ActiveGrowthRecordEditEvent(int tenantId) : base(tenantId)
        { }

        public long GrowthRecordId { get; set; }
    }
}
