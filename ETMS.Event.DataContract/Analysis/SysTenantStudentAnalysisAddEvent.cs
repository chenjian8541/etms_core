using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SysTenantStudentAnalysisAddEvent : Event
    {
        public SysTenantStudentAnalysisAddEvent(int tenantId) : base(tenantId)
        { }

        public long AddStudentId { get; set; }

        public string AddPhone { get; set; }

        public bool IsRefreshCache { get; set; }
    }
}
