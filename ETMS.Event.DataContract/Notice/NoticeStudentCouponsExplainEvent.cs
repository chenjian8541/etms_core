using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentCouponsExplainEvent : Event
    {
        public NoticeStudentCouponsExplainEvent(int tenantId) : base(tenantId)
        { }
    }
}
