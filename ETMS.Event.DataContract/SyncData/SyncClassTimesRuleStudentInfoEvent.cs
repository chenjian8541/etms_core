using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncClassTimesRuleStudentInfoEvent : Event
    {
        public SyncClassTimesRuleStudentInfoEvent(int tenantId) : base(tenantId)
        { }

        public long ClassId { get; set; }

        public long RuleId { get; set; }
    }
}
