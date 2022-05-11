using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent5.Output
{
    public class StudentReservation1v1InitOutput
    {
        public List<StudentReservation1v1Course> AllCourses { get; set; }
    }

    public class StudentReservation1v1Course
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public List<StudentReservation1v1Teacher> Teachers { get; set; }
    }

    public class StudentReservation1v1Teacher
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public long ClassId { get; set; }
    }
}
