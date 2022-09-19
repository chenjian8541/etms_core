using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeSendArrearageBatchEvent : Event
    {
        public NoticeSendArrearageBatchEvent(int tenantId) : base(tenantId)
        { }

        public List<long> OrderIds { get; set; }
    }
}
