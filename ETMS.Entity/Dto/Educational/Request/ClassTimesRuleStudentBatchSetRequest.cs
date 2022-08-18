using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleStudentBatchSetRequest : RequestBase
    {
        public long ClassId { get; set; }

        public List<long> RuleIds { get; set; }

        public List<ClassTimesRuleStudentBatchSetItem> StudentItems { get; set; }

        public override string Validate()
        {
            if (ClassId <= 0)
            {
                return "请求数据格式错误";
            }
            if (RuleIds == null || RuleIds.Count == 0)
            {
                return "请选择排课记录";
            }
            return string.Empty;
        }
    }

    public class ClassTimesRuleStudentBatchSetItem
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public string StudentName { get; set; }
    }
}
