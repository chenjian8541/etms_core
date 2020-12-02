using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeTeacherOfClassTodayClassTimesEvent : Event
    {
        public NoticeTeacherOfClassTodayClassTimesEvent(int tenantId) : base(tenantId)
        { }

        public long ClassTimesId { get; set; }

        public bool IsSendSms { get; set; }

        public bool IsSendWeChat { get; set; }

        public string WeChatNoticeRemark { get; set; }
    }
}
