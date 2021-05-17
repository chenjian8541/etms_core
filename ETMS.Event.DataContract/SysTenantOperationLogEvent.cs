using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SysTenantOperationLogEvent : Event
    {
        public SysTenantOperationLogEvent(int tenantId) : base(tenantId)
        { }

        public EtUserOperationLog UserOperationLog { get; set; }
    }
}
