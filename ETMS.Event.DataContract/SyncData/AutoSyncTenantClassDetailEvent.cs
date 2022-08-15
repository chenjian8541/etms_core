using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class AutoSyncTenantClassDetailEvent : Event
    {
        public AutoSyncTenantClassDetailEvent(int tenantId) : base(tenantId)
        { }

        public EtClass MyClass { get; set; }

        public long ClassId { get; set; }
    }
}
