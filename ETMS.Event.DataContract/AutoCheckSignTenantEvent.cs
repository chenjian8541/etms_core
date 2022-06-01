using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class AutoCheckSignTenantEvent : Event
    {
        public AutoCheckSignTenantEvent(int tenantId) : base(tenantId)
        {
        }
    }
}
