using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleChangeDataTypeBatchRequest : RequestBase
    {
        public List<long> ClassRuleIds { get; set; }

        public byte NewDataType { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (ClassRuleIds == null || ClassRuleIds.Count == 0)
            {
                return "数据格式错误";
            }
            return string.Empty;
        }
    }
}
