using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentCouponsGetEvent : Event
    {
        public NoticeStudentCouponsGetEvent(int tenantId) : base(tenantId)
        { }

        public string GenerateNo { get; set; }
    }
}
