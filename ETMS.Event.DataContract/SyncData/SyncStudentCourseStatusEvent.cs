using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncStudentCourseStatusEvent : Event
    {
        public SyncStudentCourseStatusEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }
    }
}
