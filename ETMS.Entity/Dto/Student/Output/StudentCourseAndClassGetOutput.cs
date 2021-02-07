using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseAndClassGetOutput
    {
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public List<MyStudentCourseInfo> StudentCourses { get; set; }
    }

    public class MyStudentCourseInfo
    {
        public long CourseId { get; set; }

        public byte Type { get; set; }

        public string CourseName { get; set; }

        public string SurplusQuantityDesc { get; set; }

        public bool IsActive { get; set; }

        public List<MyStudentCourseInClass> StudentCourseInClass { get; set; }
    }

    public class MyStudentCourseInClass
    {
        public long ClassId { get; set; }

        public string ClassName { get; set; }

        public string TeachersDesc { get; set; }
    }
}
