using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfHomeworkEditEvent: Event
    {
        public NoticeStudentsOfHomeworkEditEvent(int tenantId) : base(tenantId)
        { }

        public long HomeworkId { get; set; }
    }
}
