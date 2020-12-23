using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsClassEvent : Event
    {
        public StatisticsClassEvent(int tenantId) : base(tenantId)
        { }

        public EtClassRecord ClassRecord { get; set; }

        public long RecordId { get; set; }
    }
}
