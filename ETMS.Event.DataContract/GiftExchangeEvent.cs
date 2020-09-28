using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class GiftExchangeEvent : Event
    {
        public GiftExchangeEvent(int tenantId) : base(tenantId)
        { }
        public EtGiftExchangeLog giftExchangeLog { get; set; }

        public List<EtGiftExchangeLogDetail> GiftExchangeLogDetails { get; set; }
    }
}
