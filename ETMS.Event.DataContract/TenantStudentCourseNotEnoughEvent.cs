using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class TenantStudentCourseNotEnoughEvent : Event
    {
        public TenantStudentCourseNotEnoughEvent(int tenantId) : base(tenantId)
        { }
    }
}
