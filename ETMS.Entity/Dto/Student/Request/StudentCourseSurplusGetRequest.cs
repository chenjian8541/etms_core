using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseSurplusGetRequest : RequestBase
    {
        public List<MultiSelectValueRequest> StudentIds { get; set; }

        public long CourseId { get; set; }

        public byte StudentType { get; set; }

        public override string Validate()
        {
            if (StudentIds == null || !StudentIds.Any())
            {
                return "请选择学员";
            }
            if (CourseId <= 0)
            {
                return "请选择课程";
            }
            if (StudentIds.Count > 50)
            {
                return "一次性最多添加50名学员";
            }
            return string.Empty;
        }
    }
}
