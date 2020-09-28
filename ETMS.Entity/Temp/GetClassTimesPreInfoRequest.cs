using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class GetClassTimesPreInfoRequest
    {
        public EtClass EtClass { get; set; }

        public List<EtClassStudent> EtClassStudents { get; set; }

        public List<long> ClassRoomIds { get; set; }

        public List<long> TeacherIds { get; set; }

        public List<long> CourseIds { get; set; }
    }
}
