using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SysTenantStudentAnalysisRemoveEvent : Event
    {
        public SysTenantStudentAnalysisRemoveEvent(int tenantId) : base(tenantId)
        { }

        public string RemovePhone { get; set; }
    }
}
