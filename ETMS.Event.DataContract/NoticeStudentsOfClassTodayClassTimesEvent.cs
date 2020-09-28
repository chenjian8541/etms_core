using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfClassTodayClassTimesEvent : Event
    {
        public NoticeStudentsOfClassTodayClassTimesEvent(int tenantId) : base(tenantId)
        { }

        public long ClassTimesId { get; set; }

        public bool IsSendSms { get; set; }

        public bool IsSendWeChat { get; set; }
    }
}

