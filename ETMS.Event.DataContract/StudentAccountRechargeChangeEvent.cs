using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentAccountRechargeChangeEvent : Event
    {
        public StudentAccountRechargeChangeEvent(int tenantId) : base(tenantId)
        { }

        public decimal AddBalanceReal { get; set; }

        public decimal AddBalanceGive { get; set; }

        public decimal AddRechargeSum { get; set; }

        public decimal AddRechargeGiveSum { get; set; }

        public long StudentAccountRechargeId { get; set; }
    }
}
