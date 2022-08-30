using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseGiveClassTimesRequest : RequestBase
    {
        public List<long> StudentIds { get; set; }

        public long CourseId { get; set; }

        public int GiveClassTimes { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "请选择学员";
            }
            if (StudentIds.Count > 20)
            {
                return "一次性最多处理20名学员";
            }
            if (CourseId <= 0)
            {
                return "请选择课程";
            }
            if (GiveClassTimes < 0)
            {
                return "请输入赠送课时";
            }
            return string.Empty;
        }
    }


}
