using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class SendStudentCourseNotEnoughBatchRequest : RequestBase
    {
        public List<SendStudentCourseNotEnoughBatchItem> StudentCourses { get; set; }

        public override string Validate()
        {
            if (StudentCourses == null || StudentCourses.Count == 0)
            {
                return "请选择课程";
            }
            return string.Empty;
        }
    }

    public class SendStudentCourseNotEnoughBatchItem
    {
        public long CourseId { get; set; }

        public long StudentId { get; set; }
    }
}
