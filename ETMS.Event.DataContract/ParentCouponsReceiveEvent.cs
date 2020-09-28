using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ParentCouponsReceiveEvent : Event
    {
        public ParentCouponsReceiveEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }

        public long CouponsId { get; set; }
    }
}
