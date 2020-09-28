using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfClassBeforeDayTenantEvent: Event
    {
        public NoticeStudentsOfClassBeforeDayTenantEvent(int tenantId) : base(tenantId)
        { }

        public DateTime ClassOt { get; set; }

        public int NowTime { get; set; }
    }
}
