using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class TenantAgentStatisticsEvent : Event
    {
        public TenantAgentStatisticsEvent(int tenantId) : base(tenantId)
        {
        }
    }
}

