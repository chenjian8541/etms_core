using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeUserContractsNotArrivedEvent : Event
    {
        public NoticeUserContractsNotArrivedEvent(int tenantId) : base(tenantId)
        { }

        public string ClassName { get; set; }

        public EtClassRecord ClassRecord { get; set; }

        public List<EtClassRecordStudent> ClassRecordNotArrivedStudents { get; set; }
    }
}
