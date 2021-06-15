using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentAutoMarkGraduationEvent : Event
    {
        public StudentAutoMarkGraduationEvent(int tenantId) : base(tenantId)
        {
        }

        public long StudentId { get; set; }
    }
}

