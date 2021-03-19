using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassViewGetRequest : RequestBase
    {
        public long CId { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}

