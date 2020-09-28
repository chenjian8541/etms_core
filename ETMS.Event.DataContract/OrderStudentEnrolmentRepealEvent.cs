using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class OrderStudentEnrolmentRepealEvent : Event
    {
        public OrderStudentEnrolmentRepealEvent(int tenantId) : base(tenantId)
        { }

        public long OrderId { get; set; }

        public string Remark { get; set; }
    }
}

