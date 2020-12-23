using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeTeacherOfHomeworkFinishEvent : Event
    {
        public NoticeTeacherOfHomeworkFinishEvent(int tenantId) : base(tenantId)
        { }

        public long HomeworkDetailId { get; set; }
    }
}
