using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleStudentBatchSet2Request : RequestBase
    {
        public List<long> RuleIds { get; set; }

        public long ClassId { get; set; }

        public List<MultiSelectValueRequest> StudentIds { get; set; }

        public long CourseId { get; set; }

        public override string Validate()
        {
            if (ClassId <= 0 || CourseId <= 0)
            {
                return "请求数据格式错误";
            }
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "请选择学员";
            }
            if (RuleIds == null || RuleIds.Count == 0)
            {
                return "请选择排课记录";
            }
            return string.Empty;
        }
    }
}
