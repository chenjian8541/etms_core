using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Activity
{
    public class ActivityAutoRefundRouteEvent : Event
    {
        public ActivityAutoRefundRouteEvent(int tenantId) : base(tenantId)
        { }

        public long ActivityRouteId { get; set; }
    }
}
