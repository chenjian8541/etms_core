using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SyncClassTimesStudentEvent : Event
    {
        public SyncClassTimesStudentEvent(int tenantId) : base(tenantId)
        {
        }

        public long ClassTimesId { get; set; }
    }
}

