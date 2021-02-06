using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SystemDataBackupsEvent : Event
    {
        public SystemDataBackupsEvent(int tenantId) : base(tenantId)
        {
        }
    }
}
