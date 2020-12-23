using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfMakeupEvent : Event
    {
        public NoticeStudentsOfMakeupEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }

        public long ClassTimesId { get; set; }

        public long CourseId { get; set; }
    }
}
