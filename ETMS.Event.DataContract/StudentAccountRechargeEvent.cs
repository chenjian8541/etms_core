using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentAccountRechargeEvent : Event
    {
        public StudentAccountRechargeEvent(int tenantId) : base(tenantId)
        {
        }
        public EtStudentAccountRecharge AccountLog { get; set; }
        public StudentAccountRechargeRequest RechargeRequest { get; set; }

        public string No { get; set; }

        public DateTime CreateOt { get; set; }
    }
}