using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ResetTenantToDoThingEvent : Event
    {
        public ResetTenantToDoThingEvent(int tenantId) : base(tenantId)
        { }
    }
}
