using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SysTenantUserAnalysisRemoveEvent : Event
    {
        public SysTenantUserAnalysisRemoveEvent(int tenantId) : base(tenantId)
        { }

        public string RemovePhone { get; set; }
    }
}

