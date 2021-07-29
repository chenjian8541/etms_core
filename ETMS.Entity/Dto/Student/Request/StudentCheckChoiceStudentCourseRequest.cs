using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCheckChoiceStudentCourseRequest : RequestBase
    {
        public long CourseId { get; set; }

        public long StudentCheckOnLogId { get; set; }

        public override string Validate()
        {
            if (CourseId == 0 || StudentCheckOnLogId == 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}