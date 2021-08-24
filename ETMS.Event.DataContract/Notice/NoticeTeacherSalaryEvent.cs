using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class NoticeTeacherSalaryEvent : Event
    {
        public NoticeTeacherSalaryEvent(int tenantId) : base(tenantId)
        { }

        public long PayrollId { get; set; }
    }
}
