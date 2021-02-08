using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SysTenantStudentAnalysisEvent : Event
    {
        public SysTenantStudentAnalysisEvent(int tenantId) : base(tenantId)
        { }
    }
}
