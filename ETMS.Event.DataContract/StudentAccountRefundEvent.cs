using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentAccountRefundEvent : Event
    {
        public StudentAccountRefundEvent(int tenantId) : base(tenantId)
        {
        }
        public EtOrder Order { get; set; }
        public StudentAccountRefundRequest RefundRequest { get; set; }

        public EtStudentAccountRecharge AccountLog { get; set; }

        public string No { get; set; }

        public DateTime CreateOt { get; set; }
    }
}
