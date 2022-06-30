using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Activity
{
    public class SuixingRefundCallbackEvent : Event
    {
        public SuixingRefundCallbackEvent(int tenantId) : base(tenantId)
        { }

        public long ActivityRouteItemId { get; set; }

        public DateTime RefundTime { get; set; }
    }
}
