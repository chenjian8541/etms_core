using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SysTenantUserAnalysisEvent : Event
    {
        public SysTenantUserAnalysisEvent(int tenantId) : base(tenantId)
        { }
    }
}
