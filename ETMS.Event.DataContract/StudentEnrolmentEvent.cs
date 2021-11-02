using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentEnrolmentEvent : Event
    {
        public StudentEnrolmentEvent(int tenantId) : base(tenantId)
        { }

        public EtOrder Order { get; set; }

        public List<EtOrderDetail> OrderDetails { get; set; }

        public List<EtStudentCourseDetail> StudentCourseDetails { get; set; }

        public List<EtIncomeLog> IncomeLogs { get; set; }

        public List<OneToOneClass> OneToOneClassList { get; set; }

        public List<long> CouponsStudentGetIds { get; set; }

        public bool IsMallOrder { get; set; }
    }
}
