using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassStudentGetRequest : RequestBase
    {
        public long ClassId { get; set; }

        public override string Validate()
        {
            if (ClassId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}

