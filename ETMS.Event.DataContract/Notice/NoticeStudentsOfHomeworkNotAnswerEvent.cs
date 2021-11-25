using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfHomeworkNotAnswerEvent : Event
    {
        public NoticeStudentsOfHomeworkNotAnswerEvent(int tenantId) : base(tenantId)
        { }

        public DateTime MyNow { get; set; }
    }
}
