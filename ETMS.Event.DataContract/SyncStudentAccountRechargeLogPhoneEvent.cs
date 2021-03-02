using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class SyncStudentAccountRechargeLogPhoneEvent : Event
    {
        public SyncStudentAccountRechargeLogPhoneEvent(int tenantId) : base(tenantId)
        {
        }

        public long StudentAccountRechargeId { get; set; }

        public string NewPhone { get; set; }
    }
}
