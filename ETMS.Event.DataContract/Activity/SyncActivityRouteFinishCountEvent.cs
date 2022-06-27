using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Activity
{
    public class SyncActivityRouteFinishCountEvent : Event
    {
        public SyncActivityRouteFinishCountEvent(int tenantId) : base(tenantId)
        { }

        public int ActivityId { get; set; }

        public int CountLimit { get; set; }

        public long ActivityRouteId { get; set; }

        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }
    }
}
