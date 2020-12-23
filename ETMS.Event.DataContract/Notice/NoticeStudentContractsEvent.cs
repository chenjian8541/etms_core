using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentContractsEvent: Event
    {
        public NoticeStudentContractsEvent(int tenantId) : base(tenantId)
        { }

        public EtOrder Order { get; set; }

        public List<EtOrderDetail> OrderDetails { get; set; }
    }
}
