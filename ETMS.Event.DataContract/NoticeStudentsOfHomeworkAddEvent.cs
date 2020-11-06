using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfHomeworkAddEvent: Event
    {
        public NoticeStudentsOfHomeworkAddEvent(int tenantId) : base(tenantId)
        { }

        public long HomeworkId { get; set; }
    }
}
