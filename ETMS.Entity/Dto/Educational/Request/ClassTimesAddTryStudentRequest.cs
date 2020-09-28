using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesAddTryStudentRequest : RequestBase
    {
        public long CId { get; set; }

        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public override string Validate()
        {
            if (CId <= 0 || CourseId <= 0)
            {
                return "请求数据不合法";
            }
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            if (CourseId <= 0)
            {
                return "请选择消耗的课程";
            }
            return string.Empty;
        }
    }
}