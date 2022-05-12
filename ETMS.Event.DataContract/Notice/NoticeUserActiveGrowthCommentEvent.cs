using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeUserActiveGrowthCommentEvent : Event
    {
        public NoticeUserActiveGrowthCommentEvent(int tenantId) : base(tenantId)
        { }

        public EtActiveGrowthRecordDetailComment ActiveGrowthRecordDetailComment { get; set; }

        public long StudentId { get; set; }
    }
}
