using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseDetailGet2Output
    {
        public bool IsShowSetStudentCheckDefault { get; set; }

        public List<StudentCourseDetailGetOutput> CourseItems { get; set; }
    }
}
