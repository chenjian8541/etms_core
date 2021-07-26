using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseStopRequest : RequestBase
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public string CourseName { get; set; }

        public DateTime? RestoreTime { get; set; }

        public string Remark { get; set; }
        public override string Validate()
        {
            if (StudentId <= 0 || CourseId <= 0)
            {
                return "请求数据格式错误";
            }
            if (RestoreTime != null)
            {
                if (!RestoreTime.Value.IsEffectiveDate())
                {
                    return "复课日期格式错误";
                }
                if (RestoreTime.Value <= DateTime.Now)
                {
                    return "复课日期必须大于当前时间";
                }
            }
            return string.Empty;
        }
    }
}
