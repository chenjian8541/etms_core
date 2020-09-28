using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentLeaveApplyEvent : Event
    {
        public NoticeStudentLeaveApplyEvent(int tenantId) : base(tenantId)
        { }

        public EtStudentLeaveApplyLog StudentLeaveApplyLog { get; set; }
    }
}
