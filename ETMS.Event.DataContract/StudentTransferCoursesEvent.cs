using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Temp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentTransferCoursesEvent : Event
    {
        public StudentTransferCoursesEvent(int tenantId) : base(tenantId)
        { }

        public List<EtStudentCourseDetail> StudentCourseDetails { get; set; }

        public List<OneToOneClass> OneToOneClassList { get; set; }

        public EtOrder TransferOrder { get; set; }

        public TransferCoursesRequest Request { get; set; }
    }
}
