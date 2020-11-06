using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfHomeworkAddCommentEvent: Event
    {
        public NoticeStudentsOfHomeworkAddCommentEvent(int tenantId) : base(tenantId)
        { }

        public long HomeworkDetailId { get; set; }

        public long AddUserId { get; set; }

        public DateTime MyOt { get; set; }
    }
}
