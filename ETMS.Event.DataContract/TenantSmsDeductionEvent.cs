using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class TenantSmsDeductionEvent : Event
    {
        public TenantSmsDeductionEvent(int tenantId) : base(tenantId)
        { }

        public List<EtStudentSmsLog> StudentSmsLogs { get; set; }

        public List<EtUserSmsLog> UserSmsLogs { get; set; }
    }
}
