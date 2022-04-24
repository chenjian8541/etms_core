using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Statistics
{
    public class SysTenantStatistics2Event : Event
    {
        public SysTenantStatistics2Event(int tenantId) : base(tenantId)
        { }
    }
}
