using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class NoticeUserAboutStudentCheckOnEvent : Event
    {
        public NoticeUserAboutStudentCheckOnEvent(int tenantId) : base(tenantId)
        { }

        public long StudentCheckOnLogId { get; set; }
    }
}

