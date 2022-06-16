using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Activity
{
    public class SyncActivityBascInfoEvent : Event
    {
        public SyncActivityBascInfoEvent(int tenantId) : base(tenantId)
        { }

        public EtActivityMain NewActivityMain { get; set; }
    }
}
