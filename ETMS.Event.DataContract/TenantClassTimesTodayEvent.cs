using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class TenantClassTimesTodayEvent : Event
    {
        public TenantClassTimesTodayEvent(int tenantId) : base(tenantId)
        { }

        public DateTime ClassOt { get; set; }
    }
}

