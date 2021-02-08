using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SysTenantUserAnalysisAddEvent : Event
    {
        public SysTenantUserAnalysisAddEvent(int tenantId) : base(tenantId)
        { }

        public long AddUserId { get; set; }

        public string AddPhone { get; set; }

        public bool IsRefreshCache { get; set; }
    }
}
