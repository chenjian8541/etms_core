using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeTeacherOfClassTodayTenantEvent : Event
    {
        public NoticeTeacherOfClassTodayTenantEvent(int tenantId) : base(tenantId)
        { }

        public DateTime ClassOt { get; set; }

        /// <summary>
        /// 当前时间 (秒数为0)
        /// </summary>
        public DateTime NowTime { get; set; }
    }
}
