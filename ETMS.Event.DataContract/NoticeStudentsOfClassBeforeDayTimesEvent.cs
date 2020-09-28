using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfClassBeforeDayTimesEvent : Event
    {
        public NoticeStudentsOfClassBeforeDayTimesEvent(int tenantId) : base(tenantId)
        { }

        public EtClassTimes ClassTimes { get; set; }

        public bool IsSendSms { get; set; }

        public bool IsSendWeChat { get; set; }
    }
}
