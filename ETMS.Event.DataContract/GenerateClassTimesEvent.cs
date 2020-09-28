using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class GenerateClassTimesEvent: Event
    {
        public GenerateClassTimesEvent(int tenantId) : base(tenantId)
        { }

        public long ClassTimesRuleId { get; set; }

        public long ClassId { get; set; }
    }
}
