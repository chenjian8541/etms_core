using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleStudentAdd2Request : RequestBase
    {
        public long RuleId { get; set; }

        public long ClassId { get; set; }

        public List<MultiSelectValueRequest> StudentIds { get; set; }

        public long CourseId { get; set; }

        public override string Validate()
        {
            if (RuleId <= 0 || ClassId <= 0)
            {
                return "请求数据格式错误";
            }
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "请选择学员";
            }
            return string.Empty;
        }
    }
}
