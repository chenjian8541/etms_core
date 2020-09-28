using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassRecordGetRequest : RequestBase
    {
        public long ClassRecordId { get; set; }

        public override string Validate()
        {
            if (ClassRecordId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}

