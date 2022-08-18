using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleStudentAddRequest : RequestBase
    {
        public long RuleId { get; set; }

        public long ClassId { get; set; }

        public List<ClassTimesRuleStudentAddItem> StudentItems { get; set; }

        public override string Validate()
        {
            if (RuleId <= 0 || ClassId <= 0)
            {
                return "请求数据格式错误";
            }
            if (StudentItems == null || StudentItems.Count == 0)
            {
                return "请选择学员";
            }
            return string.Empty;
        }
    }

    public class ClassTimesRuleStudentAddItem
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public string StudentName { get; set; }
    }
}