using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Activity
{
    public class ActivityAutoRefundTenantEvent : Event
    {
        public ActivityAutoRefundTenantEvent(int tenantId) : base(tenantId)
        { }
    }
}
