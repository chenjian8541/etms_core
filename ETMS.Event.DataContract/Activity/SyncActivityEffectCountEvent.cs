using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Activity
{
    public class SyncActivityEffectCountEvent : Event
    {
        public SyncActivityEffectCountEvent(int tenantId) : base(tenantId)
        { }

        public long ActivityId { get; set; }

        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }
    }
}
