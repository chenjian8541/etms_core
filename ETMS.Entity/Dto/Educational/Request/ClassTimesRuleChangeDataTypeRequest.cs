using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleChangeDataTypeRequest : RequestBase
    {
        public long ClassRuleId { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (ClassRuleId <= 0)
            {
                return "数据格式错误";
            }
            return string.Empty;
        }
    }
}
