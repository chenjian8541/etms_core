using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseRelationClassRequest : RequestBase
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public string ClassName { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            if (CourseId <= 0)
            {
                return "请先选择课程";
            }
            return string.Empty;
        }
    }
}