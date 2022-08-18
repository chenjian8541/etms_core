using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleStudentGetRequest : RequestBase
    {
        public long ClassId { get; set; }

        public long RuleId { get; set; }

        public override string Validate()
        {
            if (ClassId <= 0 || RuleId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
