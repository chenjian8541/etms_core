using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentClassCheckSignRevokeEvent : Event
    {
        public NoticeStudentClassCheckSignRevokeEvent(int tenantId) : base(tenantId)
        { }

        public EtClassRecord ClassRecord { get; set; }

        public List<EtClassRecordStudent> ClassRecordStudent { get; set; }
    }
}
