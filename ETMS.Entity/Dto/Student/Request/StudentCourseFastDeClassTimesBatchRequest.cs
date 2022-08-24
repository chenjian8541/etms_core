using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseFastDeClassTimesBatchRequest : RequestBase
    {
        public List<long> StudentIds { get; set; }

        public long CourseId { get; set; }

        public decimal DeClassTimes { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (CourseId <= 0)
            {
                return "请选择课程";
            }
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "请选择学员";
            }
            if (StudentIds.Count > 100)
            {
                return "一次性最多处理100位学员";
            }
            if (DeClassTimes < 0)
            {
                return "请输入扣的课时";
            }
            return string.Empty;
        }
    }
}
