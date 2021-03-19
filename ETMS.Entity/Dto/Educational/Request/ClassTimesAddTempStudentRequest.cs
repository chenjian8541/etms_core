using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Entity.Dto.Common.Request;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesAddTempStudentRequest : RequestBase
    {
        public long CId { get; set; }

        public List<MultiSelectValueRequest> StudentIds { get; set; }

        public long CourseId { get; set; }

        public override string Validate()
        {
            if (CId <= 0 || CourseId <= 0)
            {
                return "请求数据格式错误";
            }
            if (StudentIds == null || !StudentIds.Any())
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