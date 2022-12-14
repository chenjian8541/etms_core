using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseSetIsLimitReserveRequest : RequestBase
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public bool IsLimitReserve { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0 || CourseId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}

