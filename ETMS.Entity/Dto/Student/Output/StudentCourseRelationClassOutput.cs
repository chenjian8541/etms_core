using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseRelationClassOutput
    {
        public long ClassId { get; set; }

        public string Name { get; set; }

        public int StudentNums { get; set; }

        public int? LimitStudentNums { get; set; }

        public string TeachersDesc { get; set; }

        public string CourseListDesc { get; set; }

        public bool IsDefaultInClass { get; set; }
    }
}
