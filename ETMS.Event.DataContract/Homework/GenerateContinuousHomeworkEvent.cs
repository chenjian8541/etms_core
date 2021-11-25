using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class GenerateContinuousHomeworkEvent : Event
    {
        public GenerateContinuousHomeworkEvent(int tenantId) : base(tenantId)
        { }

        public DateTime MyDate { get; set; }
    }
}
