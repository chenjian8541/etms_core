using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfWxMessageEvent : Event
    {
        public NoticeStudentsOfWxMessageEvent(int tenantId) : base(tenantId)
        { }

        public List<long> StudentIds { get; set; }

        public long WxMessageAddId { get; set; }
    }
}
