using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class ClassCheckSignRevokeEvent : Event
    {
        public ClassCheckSignRevokeEvent(int tenantId) : base(tenantId)
        { }

        public long ClassRecordId { get; set; }
    }
}

