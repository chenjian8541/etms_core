using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeUserOfStudentTryClassFinishEvent : Event
    {
        public NoticeUserOfStudentTryClassFinishEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public EtClassRecord ClassRecord { get; set; }
    }
}
