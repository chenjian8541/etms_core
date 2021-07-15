using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SyncStudentClassInfoEvent : Event
    {
        public SyncStudentClassInfoEvent(int tenantId) : base(tenantId)
        {
        }

        public long StudentId { get; set; }
    }
}


