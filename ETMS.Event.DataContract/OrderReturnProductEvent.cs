using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class OrderReturnProductEvent : Event
    {
        public OrderReturnProductEvent(int tenantId) : base(tenantId)
        { }

        public EtOrder SourceOrder { get; set; }

        public EtOrder NewOrder { get; set; }

        public List<EtOrderDetail> NewOrderDetails { get; set; }

        public OrderReturnProductRequest returnRequest { get; set; }
    }
}
