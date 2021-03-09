using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentAccountRechargeChangedEvent : Event
    {
        public NoticeStudentAccountRechargeChangedEvent(int tenantId) : base(tenantId)
        {
        }

        public EtStudentAccountRecharge StudentAccountRecharge { get; set; }

        public decimal AddBalanceReal { get; set; }

        public decimal AddBalanceGive { get; set; }

        public DateTime OtTime { get; set; }
    }
}
