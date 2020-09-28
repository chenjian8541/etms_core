using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentCourseGetOutput
    {
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public long CourseId { get; set; }

        public string CourseColor { get; set; }

        public string CourseName { get; set; }

        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public ParentDeTypeClassTimes DeTypeClassTimes { get; set; }

        public ParentDeTypeDay DeTypeDay { get; set; }

        public List<ParentStudentClass> StudentClass { get; set; }
    }

    /// <summary>
    /// 按课时
    /// </summary>
    public class ParentDeTypeClassTimes
    {
        public string SurplusQuantityDesc { get; set; }
    }

    /// <summary>
    /// 按天消耗
    /// </summary>
    public class ParentDeTypeDay
    {
        public string SurplusQuantityDesc { get; set; }
    }

    public class ParentStudentClass
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
