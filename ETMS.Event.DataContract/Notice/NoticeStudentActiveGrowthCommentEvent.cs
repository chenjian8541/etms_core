using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentActiveGrowthCommentEvent : Event
    {
        public NoticeStudentActiveGrowthCommentEvent(int tenantId) : base(tenantId)
        { }

        public EtActiveGrowthRecordDetailComment ActiveGrowthRecordDetailComment { get; set; }
    }
}
