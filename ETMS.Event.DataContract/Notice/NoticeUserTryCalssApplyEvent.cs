using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeUserTryCalssApplyEvent : Event
    {
        public NoticeUserTryCalssApplyEvent(int tenantId) : base(tenantId)
        { }

        public EtTryCalssApplyLog TryCalssApplyLog { get; set; }
    }
}
