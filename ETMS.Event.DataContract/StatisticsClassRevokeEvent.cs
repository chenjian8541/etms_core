using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsClassRevokeEvent : Event
    {
        public StatisticsClassRevokeEvent(int tenantId) : base(tenantId)
        { }

        public EtClassRecord ClassRecord { get; set; }
    }
}
