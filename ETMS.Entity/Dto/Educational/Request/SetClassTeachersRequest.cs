using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class SetClassTeachersRequest : RequestBase
    {
        public List<long> ClassIds { get; set; }

        public List<long> Teachers { get; set; }

        public override string Validate()
        {
            if (ClassIds == null || !ClassIds.Any() || Teachers == null || !Teachers.Any())
            {
                return "请求数据格式错误";
            }
            if (Teachers.Count > 20)
            {
                return "最多设置20位上课老师";
            }
            return string.Empty;
        }
    }
}


