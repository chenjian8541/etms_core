using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ActiveGrowthRecordAddEvent : Event
    {
        public ActiveGrowthRecordAddEvent(int tenantId) : base(tenantId)
        { }

        public long GrowthRecordId { get; set; }
    }
}
