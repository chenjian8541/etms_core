using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SyncParentStudentsEvent : Event
    {
        public SyncParentStudentsEvent(int tenantId) : base(tenantId)
        {
        }

        public string[] Phones { get; set; }
    }
}

