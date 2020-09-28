using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassStudentAddRequest : RequestBase
    {
        public long ClassId { get; set; }

        public List<MultiSelectValueRequest> StudentIds { get; set; }

        public long CourseId { get; set; }

        public override string Validate()
        {
            if (ClassId <= 0 || CourseId <= 0)
            {
                return "请求数据不合法";
            }
            if (StudentIds == null || !StudentIds.Any())
            {
                return "请选择学员";
            }
            if (StudentIds.Count > 50)
            {
                return "一次性最多添加50名学员";
            }
            return string.Empty;
        }
    }
}
