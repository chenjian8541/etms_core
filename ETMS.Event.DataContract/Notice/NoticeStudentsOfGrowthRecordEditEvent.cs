using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentsOfGrowthRecordEditEvent : Event
    {
        public NoticeStudentsOfGrowthRecordEditEvent(int tenantId) : base(tenantId)
        { }

        public long GrowthRecordId { get; set; }
    }
}
